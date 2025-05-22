using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models.ViewModels.Profit_Loss;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services;
public class ProfitAndLossService : IProfitAndLossService
{
    private readonly AppDbContext _context;

    public ProfitAndLossService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProfitAndLossViewModel> GetProfitLossAsync(DateTime fromDate, DateTime toDate, int branchId)
    {
        var incomeTypeIds = new[] { 21, 22 };
        var expenseTypeIds = new[] { 23, 24, 25 };

        var incomeQuery = _context.JournalPostDetails.Where(j => incomeTypeIds.Contains(j.ChartOfAccount!.ChartOfAccountTypeId) && j.VchDate >= fromDate && j.VchDate <= toDate);
        var expenseQuery = _context.JournalPostDetails.Where(j => expenseTypeIds.Contains(j.ChartOfAccount!.ChartOfAccountTypeId));
        if (branchId != 0)
        {
            incomeQuery = incomeQuery.Where(j => j.BranchId == branchId);
            expenseQuery = expenseQuery.Where(j => j.BranchId == branchId);
        }

        var incomeAccounts = await incomeQuery.GroupBy(j => j.ChartOfAccount!.Name)
            .Select(g => new AccountAmountViewModel
            {
                AccountName = g.Key!,
                Amount = g.Sum(j => j.Credit ?? 0)
            }).ToListAsync();

        var expenseAccounts = await expenseQuery.GroupBy(j => j.ChartOfAccount!.Name)
            .Select(g => new AccountAmountViewModel
            {
                AccountName = g.Key!,
                Amount = g.Sum(j => j.Debit ?? 0)
            }).ToListAsync();

        return new ProfitAndLossViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            IncomeAccounts = incomeAccounts,
            ExpenseAccounts = expenseAccounts
        };
    }
}