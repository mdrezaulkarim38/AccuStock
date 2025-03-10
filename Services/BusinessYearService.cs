using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using System.Security.Claims;

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
    public async Task<bool> CreateBusinessYear(BusinessYear businessYear)
    {
        try
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            businessYear.SubscriptionId = int.Parse(subscriptionIdClaim!);  
            businessYear.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);         
            await _context.BusinessYears.AddAsync(businessYear);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> UpdateBusinessYear(BusinessYear businessYear)
    {
        try
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            if (subscriptionIdClaim == null)
            {
                return false;
            }
            var existingBusinessYear = await _context.BusinessYears
                .FirstOrDefaultAsync(u => u.SubscriptionId == int.Parse(subscriptionIdClaim) && u.Id == businessYear.Id);

            if (existingBusinessYear == null)
            {
                return false;
            }

            existingBusinessYear.Name = businessYear.Name;
            existingBusinessYear.FromDate = businessYear.FromDate;
            existingBusinessYear.ToDate = businessYear.ToDate;
            existingBusinessYear.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!); 
            _context.BusinessYears.Update(existingBusinessYear);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> ToggleBusinessYearStatusAsync(int busineesyearId)
    {
        var businesyearId = await _context.BusinessYears.FindAsync(busineesyearId);
        if (businesyearId == null)
            return false;

        businesyearId.Status = !businesyearId.Status;
        _context.BusinessYears.Update(businesyearId);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<List<BusinessYear>> GetAllBusinessYear()
    {
        var subscriptionId = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
        return await _context.BusinessYears.Where(b=> b.SubscriptionId == int.Parse(subscriptionId!)).ToListAsync();
    }
}