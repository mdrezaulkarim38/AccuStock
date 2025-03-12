using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IChartOfAccount
    {
        Task<List<ChartOfAccount>> GetAllChartOfAccount();
        Task<List<ChartOfAccountType>> GetAllChartOfAccountType();
        Task<bool> CreateChartOfAccount(ChartOfAccount chartOfAccount);
        Task<bool> UpdateChartOfAccount(ChartOfAccount chartOfAccount);
    }
}
