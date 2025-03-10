using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IbankAccounts
    {
        Task<List<BankAccounts>> GetAllBannk();
        Task<bool> CreateBank(BankAccounts bank);
        Task<bool> UpdateBank(BankAccounts bank);
        Task<bool> ToggleBankStatus(int bankId);
    }
}
