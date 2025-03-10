using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Migrations;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AccuStock.Services
{
    public class BankAccountsService : IBankAccounts
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BankAccountsService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> CreateBank(BankAccounts bank)
        {
            try
            {
                var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
                bank.SubscriptionId = int.Parse(subscriptionIdClaim!);
                bank.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                await _context.BankAccounts.AddAsync(bank);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateBank(BankAccounts Bank)
        {
            try
            {
                var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
                if (subscriptionIdClaim == null)
                {
                    return false;
                }
                var existingBank = await _context.BankAccounts
                    .FirstOrDefaultAsync(b => b.SubscriptionId == int.Parse(subscriptionIdClaim) && b.Id == Bank.Id);

                if (existingBank == null)
                {
                    return false;
                }
                existingBank.BankName = Bank.BankName;
                existingBank.BranchName = Bank.BranchName;
                existingBank.RoutingNo = Bank.RoutingNo;
                existingBank.AccountNo = Bank.AccountNo;
                existingBank.Remarks = Bank.Remarks;
                existingBank.BranchId = Bank.BranchId;
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


        public async Task<List<BankAccounts>> GetAllBannk()
        {
            var subscriptionId = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            return await _context.BankAccounts.Where(b => b.SubscriptionId == int.Parse(subscriptionId!)).ToListAsync();
        }

    }
}
