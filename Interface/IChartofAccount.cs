using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IChartofAccount
    {
        Task<List<ChartOfAccount>> GetAllChartOfAccount();
        Task<List<ChartOfAccountType>> GetAllChartOfAccountType();
    }
}
