using AccuStock.Data;
using AccuStock.Models;

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

        //public async Task<List<Sale>> GetAllSale()
        //{
        //    var subscriptionIdClaim = _baseService.GetSubscriptionId();
        //    return await _context.Sales
        //        .Where(c => c.SubscriptionId == subscriptionIdClaim)
        //        .Include(c => c.Customer)
        //        .Include(c => c.Branch)
        //        .ToListAsync();
        //}
    }
}
