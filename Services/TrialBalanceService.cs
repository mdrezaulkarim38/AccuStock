using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models.ViewModels.TrialBalance;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services;
    public class TrialBalanceService : ITrialBalanceService
{
    private readonly AppDbContext _context;
    private readonly BaseService _baseService;

    public TrialBalanceService(AppDbContext context, BaseService baseService)
    {
        _context = context;
        _baseService = baseService;
    }

    public async Task<List<TrialBalanceReport>> GetTrialBalanceAsync(DateTime? startDate, DateTime? endDate, int? branchId)
    {
        var subscriptionId = _baseService.GetSubscriptionId();

        var query = _context.JournalPostDetails
            .Include(jpd => jpd.ChartOfAccount)
            .Where(jpd => jpd.SubscriptionId == subscriptionId)
            .AsQueryable();

        if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(j => j.VchDate >= startDate && j.VchDate <= endDate);
        }

        if (branchId.HasValue)
        {
            query = query.Where(j => j.BranchId == branchId);
        }

        var trialBalance = await query
            .GroupBy(j => j.ChartOfAccount!.Name)
            .Select(g => new TrialBalanceReport
            {
                AccountName = g.Key!,
                AccountCode = g.FirstOrDefault()!.ChartOfAccount!.AccountCode!,
                Debit = g.Sum(x => x.Debit ?? 0),
                Credit = g.Sum(x => x.Credit ?? 0)
            })
            .ToListAsync();

        return trialBalance;
    }
}