using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AccuStock.Services;

public class ChartOfAccountService : IChartOfAccount
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChartOfAccountService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetSubscriptionId()
    {
        var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
        return int.TryParse(subscriptionIdClaim, out var subscriptionId) ? subscriptionId : 0;
    }

    public async Task<List<ChartOfAccount>> GetAllChartOfAccount()
    {
        int subscriptionId = GetSubscriptionId();
        return await _context.ChartOfAccounts
            .Include(c => c.ChartOfAccountType)
            .Where(c => c.SubScriptionId == subscriptionId)
            .ToListAsync();
    }

    public async Task<List<ChartOfAccountType>> GetAllChartOfAccountType()
    {
        return await _context.ChartOfAccountTypes.ToListAsync();
    }

    public async Task<bool> CreateChartOfAccount(ChartOfAccount chartOfAccount)
    {
        if (chartOfAccount == null) throw new ArgumentNullException(nameof(chartOfAccount));
        try
        {
            chartOfAccount.SubScriptionId = GetSubscriptionId();
            chartOfAccount.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            _context.Add(chartOfAccount);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateChartOfAccount(ChartOfAccount chartOfAccount)
    {
        if (chartOfAccount == null) throw new ArgumentNullException(nameof(chartOfAccount));
        try
        {
            int subscriptionId = GetSubscriptionId();
            var existingCoa = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(coa => coa.SubScriptionId == subscriptionId && coa.Id == chartOfAccount.Id);

            if (existingCoa == null) return false;

            existingCoa.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            existingCoa.AccountCode = chartOfAccount.AccountCode;
            existingCoa.Name = chartOfAccount.Name;
            existingCoa.ParentId = chartOfAccount.ParentId;
            existingCoa.ChartOfAccountTypeId = chartOfAccount.ChartOfAccountTypeId;
            existingCoa.UpdatedAt = DateTime.UtcNow;

            _context.ChartOfAccounts.Update(existingCoa);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
}
