using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models.ViewModels.Profit_Loss;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class ProfitAndLossService : IProfitAndLossService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;

        public ProfitAndLossService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<ProfitAndLossViewModel> GetTrialBalanceAsync(DateTime fromDate, DateTime toDate, int branchId)
        {
            var incomeTypeIds = new[] { 21, 22 }; // Income & Other Income
            var expenseTypeIds = new[] { 23, 24, 25 }; // Expense, COGS, Other Expense

            // Get income accounts and sum their Credit values
            var incomeAccounts = await _context.JournalPostDetails
                .Where(j => incomeTypeIds.Contains(j.ChartOfAccount!.ChartOfAccountTypeId) &&
                            j.VchDate >= fromDate && j.VchDate <= toDate)
                .GroupBy(j => j.ChartOfAccount!.Name)
                .Select(g => new AccountAmountViewModel
                {
                    AccountName = g.Key!,
                    Amount = g.Sum(j => j.Credit ?? 0)
                }).ToListAsync();

            // Get expense accounts and sum their Debit values
            var expenseAccounts = await _context.JournalPostDetails
                .Where(j => expenseTypeIds.Contains(j.ChartOfAccount!.ChartOfAccountTypeId) &&
                            j.VchDate >= fromDate && j.VchDate <= toDate)
                .GroupBy(j => j.ChartOfAccount!.Name)
                .Select(g => new AccountAmountViewModel
                {
                    AccountName = g.Key!,
                    Amount = g.Sum(j => j.Debit ?? 0)
                }).ToListAsync();

            // Return the full ProfitAndLossViewModel
            return new ProfitAndLossViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                IncomeAccounts = incomeAccounts,
                ExpenseAccounts = expenseAccounts               
            };
        }
    }
}
