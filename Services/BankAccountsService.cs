using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services;
public class BankAccountsService : IBankAccountService
{
    private readonly AppDbContext _context;
    private readonly BaseService _baseService;
    public BankAccountsService(AppDbContext context, BaseService baseService)
    {
        _context = context;
        _baseService = baseService;
    }
    public async Task<bool> CreateBank(BankAccount bankAccount)
    {
        try
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            bankAccount.SubscriptionId = subscriptionIdClaim;
            bankAccount.UserId = _baseService.GetUserId();
            await _context.BankAccounts.AddAsync(bankAccount);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> UpdateBank(BankAccount bankAccount)
    {
        try
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            if (subscriptionIdClaim == 0)
            {
                return false;
            }
            var existingBank = await _context.BankAccounts
                .FirstOrDefaultAsync(b => b.SubscriptionId == subscriptionIdClaim && b.Id == bankAccount.Id);

            if (existingBank == null)
            {
                return false;
            }
            existingBank.BankName = bankAccount.BankName;
            existingBank.BranchName = bankAccount.BranchName;
            existingBank.RoutingNo = bankAccount.RoutingNo;
            existingBank.AccountNo = bankAccount.AccountNo;
            existingBank.Remarks = bankAccount.Remarks;
            existingBank.BranchId = bankAccount.BranchId;
            existingBank.UserId = _baseService.GetUserId();
            existingBank.UpdatedAt = DateTime.Now;
            _context.BankAccounts.Update(existingBank);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<bool> ToggleBankStatus(int bankId)
    {
        try
        {
            var bank = await _context.BankAccounts.FindAsync(bankId);
            if (bank == null)
                return false;

            bank.Status = !bank.Status;
            _context.BankAccounts.Update(bank);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<BankAccount>> GetAllBankAccount()
    {
        return await _context.BankAccounts.Where(b => b.SubscriptionId == _baseService.GetSubscriptionId()).ToListAsync();
    }

}