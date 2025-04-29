using AccuStock.Data;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class SaleService
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

            return await _context.Sales
                .Where(c => c.SubscriptionId == subscriptionId && c.BranchId == branchId)
                .Include(c => c.Customer)
                .Include(c => c.Branch)
                .ToListAsync();
        }

        public async Task<Sale?> GetSaleById(int saleId)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            if (branchId == 0)
                return null; 
            return await _context.Sales
                .Where(s => s.Id == saleId && s.SubscriptionId == subscriptionId && s.BranchId == branchId)
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
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();

            sale.SubscriptionId = subscriptionId;
            foreach (var detail in sale.SaleDetails!)
            {
                detail.Sale = sale;
            }

            await _context.Sales.AddAsync(sale);
            return await _context.SaveChangesAsync() > 0;
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
                        existingDetail.Rate = newDetail.Rate;
                        existingDetail.Discount = newDetail.Discount;
                        existingDetail.Tax = newDetail.Tax;
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


    }
}
