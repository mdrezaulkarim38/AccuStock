using AccuStock.Models.ViewModels.TrialBalance;

namespace AccuStock.Interface;

    public interface ITrialBalanceService
    {
    Task<List<TrialBalanceReport>> GetTrialBalanceAsync(DateTime? startDate, DateTime? endDate, int? branchId);
    }

