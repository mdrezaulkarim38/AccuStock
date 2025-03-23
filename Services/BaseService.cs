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
        var subscriptionId = _httpContextAccessor.HttpContext!.User.FindFirst("SubscriptionId")!.Value;
        return int.Parse(subscriptionId);
    }

    public int GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        return int.Parse(userId);
    }
    
}