using System.Security.Claims;
using AccuStock.Data;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services;

public class BaseService
{
    private readonly AppDbContext _appDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BaseService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _appDbContext = appDbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetSubscriptionId()
    {
        var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
        return int.TryParse(subscriptionIdClaim, out var subscriptionId) ? subscriptionId : 0;
    }

    public int GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        return int.TryParse(userId, out var id) ? id : 0;
    }
    
    public async Task<int> GetBranchId(int subscriptionId, int userId)
    {   
        var userBranch = await _appDbContext.Users.Where(u => u.Id == userId && u.SubscriptionId == subscriptionId).FirstOrDefaultAsync();
        return userBranch?.BranchId ?? 0;
    }
    public async Task<List<Unit>> GetAllUnit()
    {
        return await _appDbContext.Units.ToListAsync();
    }
    public async Task<int> GetBusinessYearId(int subscriptionId)
    {
       var bYearId= await _appDbContext.BusinessYears.Where(u=> u.SubscriptionId == subscriptionId).FirstOrDefaultAsync();
       return bYearId!.Id;
    }
    public async Task<string> GenerateVchNoAsync(int subscriptionId)
    {
        var currentYear = DateTime.Now.Year;
        var lastVchNo = await _appDbContext.JournalPosts
            .Where(j => j.SubscriptionId == subscriptionId)
            .OrderByDescending(j => j.VchNo)
            .FirstOrDefaultAsync();

        if (lastVchNo == null)
        {
            return $"{currentYear}000001";
        }

        var lastVchNoString = lastVchNo.VchNo!.ToString();
        var lastYear = int.Parse(lastVchNoString.Substring(0, 4));
        var lastNumber = int.Parse(lastVchNoString.Substring(4));

        if (lastYear == currentYear)
        {
            lastNumber++;
        }
        else
        {
            lastNumber = 1;
        }

        return $"{currentYear}{lastNumber:D6}";
    }
}