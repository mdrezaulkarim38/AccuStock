using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models.ViewModels.GeneralLedger;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services;
public class GLedgerService : IGLedger
{
    private readonly AppDbContext _context;
    private readonly BaseService _baseService;

    public GLedgerService(AppDbContext context, BaseService baseService)
    {
        _context = context;
        _baseService = baseService;
    }

    public async Task<List<GLedger>> GetAGLedgersList()
    {
        var groupData = await _context.JournalPostDetails
            .Include(jpd => jpd.ChartOfAccount)
            .Where(jpd => jpd.SubscriptionId == _baseService.GetSubscriptionId())
            .GroupBy(jpd => jpd.ChartOfAccountId)
            .Select(group => new GLedger
            {
                ChartOfAccountName = group.FirstOrDefault()!.ChartOfAccount!.Name,
                TotalDebit = group.Sum(jpd => jpd.Debit ?? 0),
                TotalCredit = group.Sum(jpd => jpd.Credit ?? 0)
            })
            .ToListAsync();

        return groupData;
    }

    public async Task<List<GLedger>> GetGLedger(DateTime? startDate, DateTime? endDate, int? branchId, int? chartOfAccountId)
    {
        var query = _context.JournalPostDetails
            .Include(jpd => jpd.ChartOfAccount)
            .Include(jpd => jpd.JournalPost)    
            .Where(jpd => jpd.SubscriptionId == _baseService.GetSubscriptionId())
            .AsQueryable();

        if (startDate != null && endDate != null)
        {
            query = query.Where(jpd => jpd.VchDate >= startDate && jpd.VchDate <= endDate);
        }

        if (branchId != null)
        {
            query = query.Where(jpd => jpd.JournalPost!.BranchId == branchId);
        }

        if (chartOfAccountId != null)
        {
            query = query.Where(jpd => jpd.ChartOfAccountId == chartOfAccountId);
        }

        var groupData = await query
            .GroupBy(jpd => jpd.ChartOfAccountId)
            .Select(group => new GLedger
            {
                ChartOfAccountName = group.FirstOrDefault()!.ChartOfAccount!.Name,
                TotalDebit = group.Sum(jpd => jpd.Debit ?? 0),
                TotalCredit = group.Sum(jpd => jpd.Credit ?? 0)
            })
            .ToListAsync();

        return groupData;
    }
}
