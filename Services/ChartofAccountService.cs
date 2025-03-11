using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class ChartofAccountService : IChartofAccount
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChartofAccountService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
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
    }
}
}
