using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class PurchaseReturnService : IpurchaseReturnService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;

        public PurchaseReturnService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }
        public async Task<List<Purchase>> GetPurchasesForReturn()
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            return await _context.Purchases
                .Include(p => p.Vendor)
                .Where(p => p.SubscriptionId == subscriptionId && p.PurchaseStatus == 1 && p.ReturnStatus == 0)
                .Select(p => new Purchase
                {
                    Id = p.Id,
                    PurchaseNo = p.PurchaseNo,
                    Vendor = new Vendor { Name = p.Vendor!.Name } // For display
                })
                .ToListAsync();
        }
        public async Task<string> GenerateReturnNo()
        {
            var lastReturn = await _context.PurchaseReturns
                .OrderByDescending(pr => pr.Id)
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
            return $"PR-{nextNumber.ToString("D3")}";
        }
        // Create Method //
        public async Task<bool> CreatePurchaseReturn(PurchaseReturn purchaseReturn)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate input
                if (purchaseReturn == null)
                    throw new ArgumentNullException(nameof(purchaseReturn), "Purchase return cannot be null.");

                if (purchaseReturn.PurchaseReturnDetails == null || !purchaseReturn.PurchaseReturnDetails.Any())
                    throw new ArgumentException("Purchase return must include at least one detail.", nameof(purchaseReturn.PurchaseReturnDetails));

                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = purchaseReturn.BranchId;

                var totalPurchasedQuantity = await _context.PurchaseDetails
                    .Where(x => x.PurchaseId == purchaseReturn.PurchaseId && x.SubscriptionId == subscriptionId)
                    .SumAsync(x => x.Quantity);

                var previouslyReturnedQuantity = await _context.PurchaseReturnDetails
                    .Where(x => x.SubscriptionId == subscriptionId && x.PurchaseDetail!.PurchaseId == purchaseReturn.PurchaseId)
                    .SumAsync(x => x.Quantity);

                var currentReturnQuantity = purchaseReturn.PurchaseReturnDetails.Sum(x => x.Quantity);

                var totalReturnedCount = previouslyReturnedQuantity + currentReturnQuantity;


                // Validate original purchase
                var originalPurchase = await _context.Purchases
                    .Include(p => p.Details)
                    .FirstOrDefaultAsync(p => p.Id == purchaseReturn.PurchaseId && p.SubscriptionId == subscriptionId);
                if (originalPurchase == null)
                    throw new ArgumentException("Original purchase not found.");
                // Set purchase return properties
                purchaseReturn.SubscriptionId = subscriptionId;
                purchaseReturn.ReturnNo = await GenerateReturnNo();
                purchaseReturn.VendorId = originalPurchase.VendorId;
                purchaseReturn.ReturnStatus = 0;

                // Validate and calculate details
                foreach (var detail in purchaseReturn.PurchaseReturnDetails)
                {
                    if (detail.Quantity <= 0 || detail.UnitPrice < 0)
                        throw new ArgumentException($"Invalid purchase return detail data for Product ID {detail.ProductId}.");

                    // Validate against original purchase detail
                    var originalDetail = originalPurchase.Details!
                        .FirstOrDefault(d => d.Id == detail.PurchaseDetailId && d.ProductId == detail.ProductId);
                    if (originalDetail == null)
                        throw new ArgumentException($"Purchase detail ID {detail.PurchaseDetailId} not found in original purchase.");

                    // Check if return quantity is valid
                    var totalReturned = await _context.PurchaseReturnDetails
                        .Where(prd => prd.PurchaseDetailId == detail.PurchaseDetailId && prd.SubscriptionId == subscriptionId)
                        .SumAsync(prd => prd.Quantity);
                    if (totalReturned + detail.Quantity > originalDetail.Quantity)
                        throw new ArgumentException($"Cannot return more than purchased quantity for Product ID {detail.ProductId}.");

                    // Calculate amounts
                    detail.SubTotal = detail.Quantity * detail.UnitPrice;
                    detail.VatAmount = detail.SubTotal * (detail.VatRate / 100);
                    detail.Total = detail.SubTotal + detail.VatAmount;
                    detail.SubscriptionId = subscriptionId;
                }

                // Calculate totals
                purchaseReturn.SubTotal = purchaseReturn.PurchaseReturnDetails.Sum(d => d.SubTotal);
                purchaseReturn.TotalVat = purchaseReturn.PurchaseReturnDetails.Sum(d => d.VatAmount);
                purchaseReturn.TotalAmount = purchaseReturn.SubTotal + purchaseReturn.TotalVat;

                // Save purchase return and update return status in purchase
                originalPurchase.ReturnStatus = totalReturnedCount >= totalPurchasedQuantity ? 1 : 0; // Mark as returned
                _context.Purchases.Update(originalPurchase);
                await _context.PurchaseReturns.AddAsync(purchaseReturn);
                await _context.SaveChangesAsync();

                // Update inventory (ProductStock)
                foreach (var detail in purchaseReturn.PurchaseReturnDetails)
                {
                    var stock = new ProductStock
                    {
                        ProductId = detail.ProductId,
                        Date = purchaseReturn.ReturnDate,
                        QuantityIn = 0,
                        QuantityOut = detail.Quantity,
                        SourceType = "PurchaseReturn",
                        ReferenceNo = purchaseReturn.ReturnNo ?? "",
                        SourceId = purchaseReturn.Id,
                        Remarks = "Stock removed due to purchase return",
                        SubscriptionId = subscriptionId
                    };
                    await _context.ProductStocks.AddAsync(stock);
                }

                await _context.SaveChangesAsync();

                // Create journal post for purchase return
                var journal = new JournalPost
                {
                    BusinessYearId = await _baseService.GetBusinessYearId(subscriptionId),
                    BranchId = branchId,
                    VchNo = await _baseService.GenerateVchNoAsync(subscriptionId),
                    VchDate = purchaseReturn.ReturnDate,
                    VchType = 8, // Purchase Return (new VchType)
                    Debit = purchaseReturn.TotalAmount,
                    Credit = purchaseReturn.TotalAmount,
                    PurchaseId = purchaseReturn.PurchaseId,
                    PurchaseReturnId = purchaseReturn.Id,
                    UserId = userId,
                    RefNo = purchaseReturn.ReturnNo,
                    Notes = "Purchase Return Entry",
                    Created = DateTime.Now,
                    SubscriptionId = subscriptionId
                };

                await _context.JournalPosts.AddAsync(journal);
                await _context.SaveChangesAsync();

                // Add journal post details (reverse of purchase)
                var journalDetails = new List<JournalPostDetail>();

                // Credit entry: Reduce inventory asset
                journalDetails.Add(new JournalPostDetail
                {
                    BusinessYearId = journal.BusinessYearId,
                    BranchId = branchId,
                    JournalPostId = journal.Id,
                    ChartOfAccountId = 22, // Inventory Stock
                    VchNo = journal.VchNo,
                    VchDate = journal.VchDate,
                    VchType = journal.VchType,
                    Credit = purchaseReturn.TotalAmount,
                    Description = "Purchase return - inventory reduction",
                    Remarks = "Inventory reduced due to purchase return",
                    PurchaseId = purchaseReturn.PurchaseId,
                    PurchaseReturnId = purchaseReturn.Id,
                    SubscriptionId = subscriptionId,
                    CreatedAt = DateTime.Now
                });

                // Debit entry: Based on original payment method
                if (originalPurchase.PaymentMethod == 0) // Credit (Accounts Payable)
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        BusinessYearId = journal.BusinessYearId,
                        BranchId = branchId,
                        JournalPostId = journal.Id,
                        ChartOfAccountId = 21, // Accounts Payable
                        VchNo = journal.VchNo,
                        VchDate = journal.VchDate,
                        VchType = journal.VchType,
                        Debit = purchaseReturn.TotalAmount,
                        Description = "Purchase return - reduce accounts payable",
                        Remarks = "Accounts payable reduced due to purchase return",
                        PurchaseId = purchaseReturn.PurchaseId,
                        PurchaseReturnId = purchaseReturn.Id,
                        SubscriptionId = subscriptionId,
                        CreatedAt = DateTime.Now
                    });
                }
                else if (originalPurchase.PaymentMethod == 1) // Cash (Refund/Receivable)
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        BusinessYearId = journal.BusinessYearId,
                        BranchId = branchId,
                        JournalPostId = journal.Id,
                        ChartOfAccountId = 23, // TODO: Set Accounts Receivable or Cash for refund
                        VchNo = journal.VchNo,
                        VchDate = journal.VchDate,
                        VchType = journal.VchType,
                        Debit = purchaseReturn.TotalAmount,
                        Description = "Purchase return - cash refund/receivable",
                        Remarks = "Refund or receivable recorded for purchase return",
                        PurchaseId = purchaseReturn.PurchaseId,
                        PurchaseReturnId = purchaseReturn.Id,
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
                Console.WriteLine($"CreatePurchaseReturn Error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred while creating the purchase return.", ex);
            }
        }
    }
}
