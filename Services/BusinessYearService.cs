using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;

namespace AccuStock.Services;
public class BusinessYearService : IBusinessYear
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;
    public BusinessYearService(IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = appDbContext;
    }
    public Task<bool> CreateBusinessYear(BusinessYear businessYear)
    {
        throw new NotImplementedException();
    }

    public async Task<List<BusinessYear>> GetAllBusinessYear()
    {
        var subscriptionId = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
        return await _context.BusinessYears.Where(b=> b.SubscriptionId == int.Parse(subscriptionId!)).ToListAsync();
    }
}