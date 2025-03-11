using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AccuStock.Services;

public class BankAccountsService : IBankAccountService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BankAccountsService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<bool> CreateBank(BankAccount bankAccount)
    {
        try
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            bankAccount.SubscriptionId = int.Parse(subscriptionIdClaim!);
            bankAccount.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            await _context.BankAccounts.AddAsync(bankAccount);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateBank(BankAccount bankAccount)
    {
        try
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            if (subscriptionIdClaim == null)
            {
                return false;
            }
            var existingBank = await _context.BankAccounts
                .FirstOrDefaultAsync(b => b.SubscriptionId == int.Parse(subscriptionIdClaim) && b.Id == bankAccount.Id);

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
            existingBank.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            _context.BankAccounts.Update(existingBank);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> ToggleBankStatus(int bankId)
    {
        var bank = await _context.BankAccounts.FindAsync(bankId);
        if (bank == null)
            return false;

        bank.Status = !bank.Status;
        _context.BankAccounts.Update(bank);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<BankAccount>> GetAllBankAccount()
    {
        var subscriptionId = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
        return await _context.BankAccounts.Where(b => b.SubscriptionId == int.Parse(subscriptionId!)).ToListAsync();
    }

}