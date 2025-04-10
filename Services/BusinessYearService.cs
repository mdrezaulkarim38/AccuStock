using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using System.Security.Claims;

namespace AccuStock.Services;
public class BusinessYearService : IBusinessYear
{
    private readonly AppDbContext _context;
    private readonly BaseService _baseService;
    public BusinessYearService(AppDbContext appDbContext, BaseService baseService)
    {
        _context = appDbContext;
        _baseService = baseService;
    }
    public async Task<bool> CreateBusinessYear(BusinessYear businessYear)
    {
        try
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            businessYear.SubscriptionId = subscriptionIdClaim; 
            businessYear.UserId = _baseService.GetUserId();
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
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            if (subscriptionIdClaim == 0)
            {
                return false;
            }
            var existingBusinessYear = await _context.BusinessYears
                .FirstOrDefaultAsync(u => u.SubscriptionId == subscriptionIdClaim && u.Id == businessYear.Id);

            if (existingBusinessYear == null)
            {
                return false;
            }

            existingBusinessYear.Name = businessYear.Name;
            existingBusinessYear.FromDate = businessYear.FromDate;
            existingBusinessYear.ToDate = businessYear.ToDate;
            existingBusinessYear.UserId = _baseService.GetUserId();
            existingBusinessYear.UpdatedAt = DateTime.Now;
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
        var businessYear = await _context.BusinessYears.FindAsync(busineesyearId);
        if (businessYear == null)
            return false;

        businessYear.Status = !businessYear.Status;
        _context.BusinessYears.Update(businessYear);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<List<BusinessYear>> GetAllBusinessYear()
    {
        return await _context.BusinessYears.Where(b=> b.SubscriptionId == _baseService.GetSubscriptionId()).ToListAsync();
    }
}