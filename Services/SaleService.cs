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

    }
}
