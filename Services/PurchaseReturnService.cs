using AccuStock.Data;

namespace AccuStock.Services
{
    public class PurchaseReturnService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;

        public PurchaseReturnService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }
    }
}
