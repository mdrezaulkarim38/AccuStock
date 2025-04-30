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

            return await _context.Purchases
                .Where(c => c.SubscriptionId == subscriptionId && c.BranchId == branchId)
                .Include(c => c.Branch)
                .ToListAsync();
        }

        public async Task<Purchase?> GetPurchasebyId(int id)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            if (branchId == 0)
                return null;
            return await _context.Purchases
                .Where(s => s.Id == id && s.SubscriptionId == subscriptionId && s.BranchId == branchId)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync();
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
        public async Task<bool> CreatePurchase(Purchase purchase)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();

            purchase.SubscriptionId = subscriptionId;
            purchase.PurchaseNo = await GeneratePurchaseNo();
            foreach (var detail in purchase.Details)
            {
                detail.SubTotal = detail.Quantity * detail.UnitPrice;
                detail.VatAmount = detail.SubTotal * (detail.VatRate / 100);
                detail.Total = detail.SubTotal + detail.VatAmount;
            }
            purchase.SubTotal = purchase.Details.Sum(d => d.SubTotal);
            purchase.TotalVat = purchase.Details.Sum(d => d.VatAmount);
            purchase.TotalAmount = purchase.SubTotal + purchase.TotalVat;
            await _context.Purchases.AddAsync(purchase);
            var result = await _context.SaveChangesAsync();
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
            return await _context.SaveChangesAsync() > 0 && result > 0;
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
                    existingPurchase.PurchaseNo = purchase.PurchaseNo;
                    existingPurchase.VendorId = purchase.VendorId;
                    existingPurchase.BranchId = purchase.BranchId;
                    existingPurchase.PurchaseStatus = purchase.PurchaseStatus;
                    existingPurchase.PurchaseDate = purchase.PurchaseDate;
                    existingPurchase.Notes = purchase.Notes;
                    existingPurchase.SubTotal = purchase.Details.Sum(d => d.SubTotal);
                    existingPurchase.TotalVat = purchase.Details.Sum(d => d.VatAmount);
                    existingPurchase.TotalAmount = purchase.SubTotal + purchase.TotalVat;

                    var existingDetails = await _context.PurchaseDetails
                        .Where(d => d.PurchaseId == purchase.Id)
                        .ToListAsync();

                    var existingProductStocks = await _context.ProductStocks
                        .Where(s => s.SourceId == purchase.Id && s.SourceType == "Purchase")
                        .ToListAsync();

                    _context.PurchaseDetails.RemoveRange(existingDetails);
                    _context.ProductStocks.RemoveRange(existingProductStocks);

                    foreach (var detail in purchase.Details)
                    {
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
                    _context.PurchaseDetails.RemoveRange(existingDetails);
                    
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
