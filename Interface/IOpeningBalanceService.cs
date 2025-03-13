using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IOpeningBalanceService
    {
        Task<List<OpeningBalances>> GetAllOpBl();
        Task<bool> CreateOpBl(OpeningBalances opbl);
        Task<bool> UpdateOpBl(OpeningBalances opbl);
        Task<bool> ToggleOpBlStatus(int opblId);
    }
}

