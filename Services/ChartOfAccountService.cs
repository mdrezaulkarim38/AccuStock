using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class ChartOfAccountService : IChartOfAccount
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChartOfAccountService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<ChartOfAccount>> GetAllChartOfAccount()
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            return await _context.ChartOfAccounts
                                 .Where(c => c.SubScriptionId == int.Parse(subscriptionIdClaim!))
                                 .ToListAsync();
        }
        public async Task<List<ChartOfAccountType>> GetAllChartOfAccountType()
        {
            return await _context.ChartOfAccountTypes.ToListAsync();
        }
    }
}

