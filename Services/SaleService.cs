using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;
        public SaleService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }
        public async Task<List<Sale>> GetAllSale()
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            //return await _context.Sales
            //    .Where(c => c.SubscriptionId == subscriptionId && c.BranchId == branchId)
            //    .Include(c => c.Customer)
            //    .Include(c => c.Branch)
            //    .ToListAsync();

            var query = _context.Sales.Include(s => s.Customer).Include(s => s.Branch).Where(s => s.SubscriptionId == subscriptionId).AsQueryable();
            return await query.ToListAsync();
        }
        public async Task<Sale?> GetSalebyId(int id)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            if (branchId == 0)
                return null;
            return await _context.Sales
                .Where(s => s.Id == id && s.SubscriptionId == subscriptionId && s.BranchId == branchId)
                .Include(s => s.Customer)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync();
        }
        public async Task<int> GetSalebyInvNum(string invoiceNumber)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            var sale = await _context.Sales
                .Where(s => s.InvoiceNumber == invoiceNumber
                            && s.SubscriptionId == subscriptionId
                            && s.BranchId == branchId)
                .FirstOrDefaultAsync();
            return sale?.Id ?? 0;
        }

        public async Task<bool> CreateSale(Sale sale)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = sale.BranchId;

                sale.SubscriptionId = subscriptionId;
                sale.InvoiceNumber = await GenerateInvoiceNo();
                if (sale.SaleDetails == null || !sale.SaleDetails.Any())
                {
                    throw new ArgumentException("SaleDetails cannot be null or empty.");
                }
                foreach (var detail in sale.SaleDetails!)
                {
                    detail.Sale = sale;
                    detail.SubTotal = detail.Quantity * detail.UnitPrice;
                    detail.VatAmount = detail.SubTotal * (detail.VatRate / 100);
                    detail.Total = detail.SubTotal + detail.VatAmount;
                    detail.SubscriptionId = subscriptionId;
                }

                sale.SubTotal = sale.SaleDetails.Sum(d => d.SubTotal);
                sale.TotalVat = sale.SaleDetails.Sum(d => d.VatAmount);
                sale.TotalAmount = sale.SubTotal + sale.TotalVat;

                await _context.Sales.AddAsync(sale);
                await _context.SaveChangesAsync();

                // Record Product Stock Out
                foreach (var detail in sale.SaleDetails)
                {
                    var currentStock = await _context.ProductStocks
                        .Where(ps => ps.ProductId == detail.ProductId && ps.SubscriptionId == subscriptionId)
                        .SumAsync(ps => ps.QuantityIn - ps.QuantityOut);

                    if (currentStock < detail.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product ID {detail.ProductId}. Available: {currentStock}, Requested: {detail.Quantity}");
                    }
                    var stock = new ProductStock
                    {
                        ProductId = detail.ProductId,
                        Date = sale.InvoiceDate,
                        QuantityOut = detail.Quantity,
                        QuantityIn = 0,
                        SourceType = "Sale",
                        ReferenceNo = sale.InvoiceNumber ?? "",
                        SourceId = sale.Id,
                        Remarks = "Stock reduced from sale",
                        SubscriptionId = subscriptionId
                    };
                    await _context.ProductStocks.AddAsync(stock);
                }
                await _context.SaveChangesAsync();
                // Journal Post
                var journal = new JournalPost
                {
                    BusinessYearId = await _baseService.GetBusinessYearId(subscriptionId),
                    BranchId = branchId,
                    VchNo = await _baseService.GenerateVchNoAsync(subscriptionId),
                    VchDate = sale.InvoiceDate,
                    VchType = 5, // Sale VCH Type (define as needed)
                    Debit = sale.TotalAmount,
                    Credit = sale.TotalAmount,
                    SaleId = sale.Id,
                    UserId = userId,
                    RefNo = sale.InvoiceNumber,
                    Notes = "Sales Entry",
                    Created = DateTime.Now,
                    SubscriptionId = subscriptionId,
                };

                await _context.JournalPosts.AddAsync(journal);
                await _context.SaveChangesAsync();

                var journalDetails = new List<JournalPostDetail>();
                if (sale.PaymentMethod == 0) // On Credit
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        ChartOfAccountId = 23, // Accounts Receivable
                        Debit = sale.TotalAmount,
                        Description = "Credit Sale - Receivable",
                        Remarks = "Accounts Receivable",
                    });
                }
                else if (sale.PaymentMethod == 1) // On Cash
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        ChartOfAccountId = 20, // Cash in Hand
                        Debit = sale.TotalAmount,
                        Description = "Cash Sale",
                        Remarks = "Cash received from sale",
                    });
                }
                // Sales Revenue Credit
                journalDetails.Add(new JournalPostDetail
                {
                    ChartOfAccountId = 24, // Sales Revenue
                    Credit = sale.TotalAmount,
                    Description = "Sales Revenue",
                    Remarks = "Credit revenue for sale"
                });
                //decimal totalCOGS = sale.SaleDetails.Sum(d => d.Quantity * (d.Product?.CostPrice ?? 0)); // You must have CostPrice in Product
                journalDetails.Add(new JournalPostDetail
                {
                    ChartOfAccountId = 25, // Cost of Goods Sold
                    Debit = sale.TotalAmount,
                    Description = "COGS Entry",
                    Remarks = "Record expense for COGS"
                });

                journalDetails.Add(new JournalPostDetail
                {
                    ChartOfAccountId = 22, // Inventory
                    Credit = sale.TotalAmount,
                    Description = "Inventory Reduction",
                    Remarks = "Reduce inventory for sold items"
                });

                // Finalize Journal Details
                foreach (var jd in journalDetails)
                {
                    jd.JournalPostId = journal.Id;
                    jd.BranchId = branchId;
                    jd.VchNo = journal.VchNo;
                    jd.VchDate = journal.VchDate;
                    jd.VchType = journal.VchType;
                    jd.BusinessYearId = journal.BusinessYearId;
                    jd.SaleId = sale.Id;
                    jd.SubscriptionId = subscriptionId;
                    jd.CreatedAt = DateTime.Now;
                }

                await _context.JournalPostDetails.AddRangeAsync(journalDetails);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"CreateSale Error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred while creating the sale.", ex);
            }
        }

        public async Task<bool> UpdateSale(Sale sale)
        {
            try
            {
                var subscriptionId = _baseService.GetSubscriptionId();

                var existingSale = await _context.Sales
                    .Include(s => s.SaleDetails)
                    .FirstOrDefaultAsync(s => s.Id == sale.Id && s.SubscriptionId == subscriptionId);

                if (existingSale == null)
                {
                    return false;
                }
                existingSale.CustomerId = sale.CustomerId;
                existingSale.BranchId = sale.BranchId;
                existingSale.InvoiceDate = sale.InvoiceDate;
                existingSale.InvoiceNumber = sale.InvoiceNumber;
                existingSale.PaymentMethod = sale.PaymentMethod;
                existingSale.PaymentStatus = sale.PaymentStatus;
                existingSale.TotalAmount = sale.TotalAmount;

                foreach (var newDetail in sale.SaleDetails!)
                {
                    var existingDetail = existingSale.SaleDetails!
                        .FirstOrDefault(d => d.Id == newDetail.Id);

                    if (existingDetail != null)
                    {
                        existingDetail.ProductId = newDetail.ProductId;
                        existingDetail.Quantity = newDetail.Quantity;
                        //existingDetail.Rate = newDetail.Rate;
                        //existingDetail.Discount = newDetail.Discount;
                        //existingDetail.Tax = newDetail.Tax;
                        _context.Entry(existingDetail).State = EntityState.Modified;
                    }
                    else
                    {
                        return false;
                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<string> DeleteSale(int saleId)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .FirstOrDefaultAsync(s => s.Id == saleId && s.SubscriptionId == subscriptionId);

            if (sale == null)
            {
                return "Sale not found.";
            }
            bool hasJournal = await _context.JournalPosts
                .AnyAsync(j => j.SaleId == saleId && j.SubscriptionId == subscriptionId);

            if (hasJournal)
            {
                return "Cannot delete sale. Journal entry already exists.";
            }
            //bool hasPayments = await _context.Receipts
            //    .AnyAsync(r => r.SaleId == saleId && r.SubscriptionId == subscriptionId);

            //if (hasPayments)
            //{
            //    return "Cannot delete sale. Payment already recorded.";
            //}
            _context.SaleDetails.RemoveRange(sale.SaleDetails!);
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            return "Sale deleted successfully.";
        }

        public async Task<string> GenerateInvoiceNo()
        {
            var lastinvoice = await _context.Sales
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync();
            int nextNumber = 1;
            if (lastinvoice != null && !string.IsNullOrEmpty(lastinvoice.InvoiceNumber))
            {
                var parts = lastinvoice.InvoiceNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"SINV{nextNumber.ToString("D2")}";
        }

    }
}
