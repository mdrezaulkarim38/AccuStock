using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IOpeningBalanceService
    {
        Task<List<OpeningBalances>> GetOpBl();
        Task<bool> CreateOpBl(OpeningBalances opbl);
        Task<bool> UpdateOpBl(OpeningBalances opbl);
    }
}

