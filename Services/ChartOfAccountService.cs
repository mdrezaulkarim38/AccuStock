using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AccuStock.Services;

public class ChartOfAccountService : IChartOfAccount
{
    private readonly AppDbContext _context;
    private readonly BaseService _baseService;

    public ChartOfAccountService(AppDbContext context, BaseService baseService)
    {
        _context = context;
        _baseService = baseService;
    }
    public async Task<List<ChartOfAccount>> GetAllChartOfAccount()
    {
        int subscriptionId = _baseService.GetSubscriptionId();
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
            chartOfAccount.SubScriptionId = _baseService.GetSubscriptionId();
            chartOfAccount.UserId = _baseService.GetUserId();

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
            int subscriptionId = _baseService.GetSubscriptionId();
            var existingCoa = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(coa => coa.SubScriptionId == subscriptionId && coa.Id == chartOfAccount.Id);

            if (existingCoa == null) return false;

            existingCoa.UserId = _baseService.GetUserId();
            existingCoa.AccountCode = chartOfAccount.AccountCode;
            existingCoa.Name = chartOfAccount.Name;
            existingCoa.ParentId = chartOfAccount.ParentId;
            existingCoa.ChartOfAccountTypeId = chartOfAccount.ChartOfAccountTypeId;
            existingCoa.UpdatedAt = DateTime.Now;

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
