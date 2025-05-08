    using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class PurchaseService: IPurchaseService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;
        public PurchaseService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }
        public async Task<List<Purchase>> GetAllPurchase()
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            var query = _context.Purchases.Include(c => c.Branch).Include(p => p.Vendor).Where(p => p.SubscriptionId == subscriptionId).AsQueryable();
            if(branchId != 0)
            {
                query = query.Where(p => p.BranchId == branchId);
            }
            return await query.ToListAsync();
        }

        public async Task<Purchase?> GetPurchasebyId(int id)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            var query = _context.Purchases.Include(c => c.Branch).Include(p=> p.Details).Include(p => p.Vendor).Where(p => p.SubscriptionId == subscriptionId && p.Id == id).AsQueryable();
            if(branchId != 0)
            {
                query = query.Where(p => p.BranchId == branchId);
            }
            return await query.FirstOrDefaultAsync();
        }
        public async Task<int> GetPurchasebyPurNum(string purchaseNum)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            var purchase = await _context.Purchases
                .Where(s => s.PurchaseNo == purchaseNum
                            && s.SubscriptionId == subscriptionId
                            && s.BranchId == branchId)
                .FirstOrDefaultAsync();
            return purchase?.Id ?? 0;
        }
        //public async Task<bool> CreatePurchase(Purchase purchase)
        //{
        //    try
        //    {
        //        if (purchase == null)
        //        {
        //            throw new ArgumentNullException(nameof(purchase), "Purchase cannot be null.");
        //        }

        //        if (purchase.Details == null || !purchase.Details.Any())
        //        {
        //            throw new ArgumentException("Purchase must include at least one detail.", nameof(purchase.Details));
        //        }
        //        var subscriptionId = _baseService.GetSubscriptionId();
        //        var userId = _baseService.GetUserId();

        //        if (subscriptionId <= 0)
        //        {
        //            throw new InvalidOperationException("Invalid subscription ID.");
        //        }

        //        purchase.SubscriptionId = subscriptionId;
        //        purchase.PurchaseNo = await GeneratePurchaseNo();

        //        foreach (var detail in purchase.Details)
        //        {
        //            if (detail.Quantity <= 0 || detail.UnitPrice < 0 || detail.VatRate < 0)
        //            {
        //                throw new ArgumentException($"Invalid purchase detail data for Product ID {detail.ProductId}.");
        //            }
        //            detail.SubTotal = detail.Quantity * detail.UnitPrice;
        //            detail.VatAmount = detail.SubTotal * (detail.VatRate / 100);
        //            detail.Total = detail.SubTotal + detail.VatAmount;
        //        }

        //        purchase.SubTotal = purchase.Details.Sum(d => d.SubTotal);
        //        purchase.TotalVat = purchase.Details.Sum(d => d.VatAmount);
        //        purchase.TotalAmount = purchase.SubTotal + purchase.TotalVat;

        //        await _context.Purchases.AddAsync(purchase);
        //        var result = await _context.SaveChangesAsync();

        //        if (result <= 0)
        //        {
        //            throw new InvalidOperationException("Failed to save purchase to the database.");
        //        }

        //        foreach (var detail in purchase.Details)
        //        {
        //            var stock = new ProductStock
        //            {
        //                ProductId = detail.ProductId,
        //                Date = purchase.PurchaseDate,
        //                QuantityIn = detail.Quantity,
        //                QuantityOut = 0,
        //                SourceType = "Purchase",
        //                ReferenceNo = purchase.PurchaseNo ?? "",
        //                SourceId = purchase.Id,
        //                Remarks = "Stock added from purchase"
        //            };
        //            await _context.ProductStocks.AddAsync(stock);
        //        }
        //        var stockResult = await _context.SaveChangesAsync();
        //        if (stockResult <= 0)
        //        {
        //            throw new InvalidOperationException("Failed to save product stock updates.");
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Unexpected error: {ex.Message}");
        //        throw new InvalidOperationException("An unexpected error occurred while creating the purchase.", ex);
        //    }
        //}

        public async Task<bool> CreatePurchase(Purchase purchase)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (purchase == null)
                    throw new ArgumentNullException(nameof(purchase), "Purchase cannot be null.");

                if (purchase.Details == null || !purchase.Details.Any())
                    throw new ArgumentException("Purchase must include at least one detail.", nameof(purchase.Details));

                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = purchase.BranchId;
                //var businessYearId = _baseService.GetBusinessYearId();

                purchase.SubscriptionId = subscriptionId;
                purchase.PurchaseNo = await GeneratePurchaseNo();

                foreach (var detail in purchase.Details)
                {
                    if (detail.Quantity <= 0 || detail.UnitPrice < 0)
                        throw new ArgumentException($"Invalid purchase detail data for Product ID {detail.ProductId}.");

                    detail.SubTotal = detail.Quantity * detail.UnitPrice;
                    detail.VatAmount = detail.SubTotal * (detail.VatRate / 100);
                    detail.Total = detail.SubTotal + detail.VatAmount;
                }

                purchase.SubTotal = purchase.Details.Sum(d => d.SubTotal);
                purchase.TotalVat = purchase.Details.Sum(d => d.VatAmount);
                purchase.TotalAmount = purchase.SubTotal + purchase.TotalVat;

                await _context.Purchases.AddAsync(purchase);
                await _context.SaveChangesAsync();

                foreach (var detail in purchase.Details)
                {
                    var stock = new ProductStock
                    {
                        ProductId = detail.ProductId,
                        Date = purchase.PurchaseDate,
                        QuantityIn = detail.Quantity,
                        QuantityOut = 0,
                        SourceType = "Purchase",
                        ReferenceNo = purchase.PurchaseNo ?? "",
                        SourceId = purchase.Id,
                        Remarks = "Stock added from purchase"
                    };
                    await _context.ProductStocks.AddAsync(stock);
                }

                await _context.SaveChangesAsync();
                // Insert into JournalPost
                var journal = new JournalPost                
                {
                    BusinessYearId = purchase.PurchaseDate.Year,
                    BranchId = branchId,
                    VchNo = purchase.PurchaseNo,
                    VchDate = purchase.PurchaseDate,
                    VchType = 1, // Purchase VCH Type
                    Debit = purchase.TotalAmount,
                    Credit = purchase.TotalAmount,
                    PurchaseId = purchase.Id,
                    UserId = userId,
                    RefNo = purchase.PurchaseNo,
                    Notes = "Purchase entry",
                    Created = DateTime.Now,
                    SubscriptionId = subscriptionId,
                };

                await _context.JournalPosts.AddAsync(journal);
                await _context.SaveChangesAsync();

                // Add JournalPostDetails based on payment method
                var journalDetails = new List<JournalPostDetail>();

                if (purchase.PaymentMethod == 0) // Cash Purchase
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        //BusinessYearId = businessYearId,
                        BranchId = branchId,
                        JournalPostId = journal.Id,
                        ChartOfAccountId = 101, // TODO: Set cash ChartOfAccountId
                        VchNo = journal.VchNo,
                        VchDate = journal.VchDate,
                        VchType = journal.VchType,
                        Credit = purchase.TotalAmount,
                        Description = "Cash Payment",
                        Remarks = "Cash paid for purchase",
                        PurchaseId = purchase.Id,
                        SubscriptionId = subscriptionId,
                        CreatedAt = DateTime.Now
                    });
                }
                else if (purchase.PaymentMethod == 1) // Credit Purchase
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        //BusinessYearId = businessYearId,
                        BranchId = branchId,
                        JournalPostId = journal.Id,
                        ChartOfAccountId = 102, // TODO: Set accounts payable ChartOfAccountId
                        Credit = purchase.TotalAmount,
                        VchNo = journal.VchNo,
                        VchDate = journal.VchDate,
                        VchType = journal.VchType,
                        Description = "Credit Purchase",
                        Remarks = "Purchase on credit",
                        PurchaseId = purchase.Id,
                        SubscriptionId = subscriptionId,
                        CreatedAt = DateTime.Now
                    });
                }

                // Debit entry for purchase expense
                journalDetails.Add(new JournalPostDetail
                {
                   // BusinessYearId = businessYearId,
                    BranchId = branchId,
                    JournalPostId = journal.Id,
                    ChartOfAccountId = 103, // TODO: Set purchase expense ChartOfAccountId
                    Debit = purchase.TotalAmount,
                    VchNo = journal.VchNo,
                    VchDate = journal.VchDate,
                    VchType = journal.VchType,
                    Description = "Purchase expense",
                    Remarks = "Expense booked for purchase",
                    PurchaseId = purchase.Id,
                    SubscriptionId = subscriptionId,
                    CreatedAt = DateTime.Now
                });

                await _context.JournalPostDetails.AddRangeAsync(journalDetails);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"CreatePurchase Error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred while creating the purchase.", ex);
            }
        }

        public async Task<string> GeneratePurchaseNo()
        {
            var lastPurchase = await _context.Purchases
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();
            int nextNumber = 1;
            if (lastPurchase != null && !string.IsNullOrEmpty(lastPurchase.PurchaseNo))
            {
                var parts = lastPurchase.PurchaseNo.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"P-BILL-{nextNumber.ToString("D2")}";
        }

        public async Task<bool> UpdatePurchase(Purchase purchase)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingPurchase = await _context.Purchases.FindAsync(purchase.Id);

                    if (existingPurchase == null)
                    {
                        return false;
                    }
                    //existingPurchase.PurchaseNo = await GeneratePurchaseNo();
                    existingPurchase.VendorId = purchase.VendorId;
                    existingPurchase.BranchId = purchase.BranchId;
                    existingPurchase.PurchaseStatus = purchase.PurchaseStatus;
                    existingPurchase.PurchaseDate = purchase.PurchaseDate;
                    existingPurchase.Notes = purchase.Notes;
                    existingPurchase.SubTotal = purchase.Details!.Sum(d => d.SubTotal);
                    existingPurchase.TotalVat = purchase.Details!.Sum(d => d.VatAmount);
                    existingPurchase.TotalAmount = purchase.SubTotal + purchase.TotalVat;

                    var existingDetails = await _context.PurchaseDetails
                        .Where(d => d.PurchaseId == purchase.Id)
                        .ToListAsync();

                    var existingProductStocks = await _context.ProductStocks
                        .Where(s => s.SourceId == purchase.Id && s.SourceType == "Purchase")
                        .ToListAsync();

                    _context.PurchaseDetails.RemoveRange(existingDetails);
                    _context.ProductStocks.RemoveRange(existingProductStocks);

                    foreach (var detail in purchase.Details!)
                    {
                        detail.PurchaseId = purchase.Id;
                        detail.SubTotal = detail.Quantity * detail.UnitPrice;
                        detail.VatAmount = detail.SubTotal * (detail.VatRate / 100);
                        detail.Total = detail.SubTotal + detail.VatAmount;
                        _context.PurchaseDetails.Add(detail);

                        var stock = new ProductStock
                        {
                            ProductId = detail.ProductId,
                            Date = purchase.PurchaseDate,
                            QuantityIn = detail.Quantity,
                            QuantityOut = 0,
                            SourceType = "Purchase",
                            ReferenceNo = purchase.PurchaseNo ?? "",
                            SourceId = purchase.Id,
                            Remarks = "Stock added from purchase"
                        };
                        _context.ProductStocks.Add(stock);
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error updating purchase: {ex.Message}");
                    return false;
                }
            }
        }
        public async Task<string> DeletePurchase(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {                    
                    var existingPurchase = await _context.Purchases
                        .Include(p => p.Details)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingPurchase == null)
                    {
                        return "Purchase not found.";
                    }                    
                    var existingDetails = existingPurchase.Details;
                    _context.PurchaseDetails.RemoveRange(existingDetails!);
                    
                    var existingProductStocks = await _context.ProductStocks
                        .Where(s => s.SourceId == id && s.SourceType == "Purchase")
                        .ToListAsync();
                    _context.ProductStocks.RemoveRange(existingProductStocks);                    
                    _context.Purchases.Remove(existingPurchase);                    
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return "Purchase deleted successfully.";
                }
                catch (Exception ex)
                {                    
                    await transaction.RollbackAsync();
                    return $"Error deleting purchase: {ex.Message}";
                }
            }}}}
