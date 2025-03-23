using System.Security.Claims;
using AccuStock.Data;

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
        return int.TryParse(userId, out var Id) ? Id : 0;
    }
    
}