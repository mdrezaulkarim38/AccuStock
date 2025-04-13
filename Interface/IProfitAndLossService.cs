using AccuStock.Models.ViewModels.Profit_Loss;

namespace AccuStock.Interface
{
    public interface IProfitAndLossService
    {
        Task<ProfitAndLossViewModel> GetTrialBalanceAsync(DateTime fromDate, DateTime toDate, int branchId);
    }
}
