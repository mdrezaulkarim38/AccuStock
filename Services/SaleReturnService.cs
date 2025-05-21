using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class SaleReturnService : ISaleReturnService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;

        public SaleReturnService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<List<Sale>> GetSalesForReturn()
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            return await _context.Sales
                .Include(s => s.Customer)
                .Where(s => s.SubscriptionId == subscriptionId && s.PaymentStatus == 1 && s.ReturnStatus == 0)
                .Select(s => new Sale
                {
                    Id = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    Customer = new Customer { Name = s.Customer!.Name }
                })
                .ToListAsync();
        }

        public async Task<string> GenerateReturnNo()
        {
            var lastReturn = await _context.SaleReturns
                .OrderByDescending(sr => sr.Id)
                .FirstOrDefaultAsync();
            int nextNumber = 1;
            if (lastReturn != null && !string.IsNullOrEmpty(lastReturn.ReturnNo))
            {
                var parts = lastReturn.ReturnNo.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"SR-{nextNumber.ToString("D3")}";
        }

        public async Task<bool> CreateSaleReturn(SaleReturn saleReturn)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate input
                if (saleReturn == null)
                    throw new ArgumentNullException(nameof(saleReturn), "Sale return cannot be null.");

                if (saleReturn.SaleReturnDetails == null || !saleReturn.SaleReturnDetails.Any())
                    throw new ArgumentException("Sale return must include at least one detail.", nameof(saleReturn.SaleReturnDetails));

                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = saleReturn.BranchId;

                var totalSaleQuantity = await _context.SaleDetails
                    .Where(x => x.SaleId == saleReturn.SaleId && x.SubscriptionId == subscriptionId)
                    .SumAsync(x => x.Quantity);

                var previouslyReturnedQuantity = await _context.SaleReturnDetails
                    .Where(x => x.SubscriptionId == subscriptionId && x.SaleDetail!.SaleId == saleReturn.SaleId)
                    .SumAsync(x => x.Quantity);

                var currentReturnQuantity = saleReturn.SaleReturnDetails.Sum(x => x.Quantity);

                var totalReturnedCount = previouslyReturnedQuantity + currentReturnQuantity;

                // Validate original sale
                var originalSale = await _context.Sales
                    .Include(s => s.SaleDetails)
                    .FirstOrDefaultAsync(s => s.Id == saleReturn.SaleId && s.SubscriptionId == subscriptionId);
                if (originalSale == null)
                    throw new ArgumentException("Original sale not found.");

                // Set sale return properties
                saleReturn.SubscriptionId = subscriptionId;
                saleReturn.ReturnNo = await GenerateReturnNo();
                saleReturn.CustomerId = originalSale.CustomerId;
                saleReturn.ReturnStatus = 0;

                // Validate and calculate details
                foreach (var detail in saleReturn.SaleReturnDetails)
                {
                    if (detail.Quantity <= 0 || detail.UnitPrice < 0)
                        throw new ArgumentException($"Invalid sale return detail data for Product ID {detail.ProductId}.");

                    // Validate against original sale detail
                    var originalDetail = originalSale.SaleDetails!
                        .FirstOrDefault(d => d.Id == detail.SaleDetailId && d.ProductId == detail.ProductId);
                    if (originalDetail == null)
                        throw new ArgumentException($"Sale detail ID {detail.SaleDetailId} not found in original sale.");

                    // Check if return quantity is valid
                    var totalReturned = await _context.SaleReturnDetails
                        .Where(srd => srd.SaleDetailId == detail.SaleDetailId && srd.SubscriptionId == subscriptionId)
                        .SumAsync(srd => srd.Quantity);
                    if (totalReturned + detail.Quantity > originalDetail.Quantity)
                        throw new ArgumentException($"Cannot return more than sold quantity for Product ID {detail.ProductId}.");

                    // Calculate amounts
                    detail.SubTotal = detail.Quantity * detail.UnitPrice;
                    detail.VatAmount = detail.SubTotal * (detail.VatRate / 100);
                    detail.Total = detail.SubTotal + detail.VatAmount;
                    detail.SubscriptionId = subscriptionId;
                }

                // Calculate totals
                saleReturn.SubTotal = saleReturn.SaleReturnDetails.Sum(d => d.SubTotal);
                saleReturn.TotalVat = saleReturn.SaleReturnDetails.Sum(d => d.VatAmount);
                saleReturn.TotalAmount = saleReturn.SubTotal + saleReturn.TotalVat;

                // Save sale return and update return status in sale
                originalSale.ReturnStatus = totalReturnedCount >= totalSaleQuantity ? 1 : 0;
                _context.Sales.Update(originalSale);
                await _context.SaleReturns.AddAsync(saleReturn);
                await _context.SaveChangesAsync();

                // Update inventory (ProductStock)
                foreach (var detail in saleReturn.SaleReturnDetails)
                {
                    var stock = new ProductStock
                    {
                        ProductId = detail.ProductId,
                        Date = saleReturn.ReturnDate,
                        QuantityIn = detail.Quantity, // Increase inventory for returns
                        QuantityOut = 0,
                        SourceType = "SaleReturn",
                        ReferenceNo = saleReturn.ReturnNo ?? "",
                        SourceId = saleReturn.Id,
                        Remarks = "Stock added due to sale return",
                        SubscriptionId = subscriptionId
                    };
                    await _context.ProductStocks.AddAsync(stock);
                }

                await _context.SaveChangesAsync();

                // Create journal post for sale return
                var journal = new JournalPost
                {
                    BusinessYearId = await _baseService.GetBusinessYearId(subscriptionId),
                    BranchId = branchId,
                    VchNo = await _baseService.GenerateVchNoAsync(subscriptionId),
                    VchDate = saleReturn.ReturnDate,
                    VchType = 9, // Sale Return (new VchType)
                    Debit = saleReturn.TotalAmount,
                    Credit = saleReturn.TotalAmount,
                    SaleId = saleReturn.SaleId,
                    SaleReturnId = saleReturn.Id,
                    UserId = userId,
                    RefNo = saleReturn.ReturnNo,
                    Notes = "Sale Return Entry",
                    Created = DateTime.Now,
                    SubscriptionId = subscriptionId
                };

                await _context.JournalPosts.AddAsync(journal);
                await _context.SaveChangesAsync();

                // Add journal post details (reverse of sale)
                var journalDetails = new List<JournalPostDetail>();

                // Debit entry: Sales Revenue (reduce revenue)
                journalDetails.Add(new JournalPostDetail
                {
                    BusinessYearId = journal.BusinessYearId,
                    BranchId = branchId,
                    JournalPostId = journal.Id,
                    ChartOfAccountId = 24, // Sales Revenue
                    VchNo = journal.VchNo,
                    VchDate = journal.VchDate,
                    VchType = journal.VchType,
                    Debit = saleReturn.TotalAmount,
                    Description = "Sale return - reduce sales revenue",
                    Remarks = "Sales revenue reduced due to sale return",
                    SaleId = saleReturn.SaleId,
                    SaleReturnId = saleReturn.Id,
                    SubscriptionId = subscriptionId,
                    CreatedAt = DateTime.Now
                });

                // Credit entry: Based on original payment method
                if (originalSale.PaymentMethod == 0) // Credit (Accounts Receivable)
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        BusinessYearId = journal.BusinessYearId,
                        BranchId = branchId,
                        JournalPostId = journal.Id,
                        ChartOfAccountId = 23, // Accounts Receivable
                        VchNo = journal.VchNo,
                        VchDate = journal.VchDate,
                        VchType = journal.VchType,
                        Credit = saleReturn.TotalAmount,
                        Description = "Sale return - reduce accounts receivable",
                        Remarks = "Accounts receivable reduced due to sale return",
                        SaleId = saleReturn.SaleId,
                        SaleReturnId = saleReturn.Id,
                        SubscriptionId = subscriptionId,
                        CreatedAt = DateTime.Now
                    });
                }
                else if (originalSale.PaymentMethod == 1) // Cash
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        BusinessYearId = journal.BusinessYearId,
                        BranchId = branchId,
                        JournalPostId = journal.Id,
                        ChartOfAccountId = 20, // Cash
                        VchNo = journal.VchNo,
                        VchDate = journal.VchDate,
                        VchType = journal.VchType,
                        Credit = saleReturn.TotalAmount,
                        Description = "Sale return - cash refund",
                        Remarks = "Cash refunded due to sale return",
                        SaleId = saleReturn.SaleId,
                        SaleReturnId = saleReturn.Id,
                        SubscriptionId = subscriptionId,
                        CreatedAt = DateTime.Now
                    });
                }
                await _context.JournalPostDetails.AddRangeAsync(journalDetails);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"CreateSaleReturn Error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred while creating the sale return.", ex);
            }
        }
    }
}
