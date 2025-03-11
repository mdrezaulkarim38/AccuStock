using AccuStock.Models;

namespace AccuStock.Interface;

public interface IBankAccountService
{
    Task<List<BankAccount>> GetAllBankAccount();
    Task<bool> CreateBank(BankAccount bank);
    Task<bool> UpdateBank(BankAccount bank);
    Task<bool> ToggleBankStatus(int bankId);
}