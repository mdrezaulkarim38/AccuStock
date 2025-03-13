using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AccuStock.Services
{
    public class OpeningBalanceService : IOpeningBalanceService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OpeningBalanceService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CreateOpBl(OpeningBalances openingBalance)
        {
            try
            {
                var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
                openingBalance.SubScriptionId = int.Parse(subscriptionIdClaim!);
                openingBalance.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                await _context.OpeningBalances.AddAsync(openingBalance);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> UpdateOpBl(OpeningBalances opbl)
        {
            try
            {
                var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
                var existingOpBl = await _context.OpeningBalances.FirstOrDefaultAsync(o => o.SubScriptionId == int.Parse(subscriptionIdClaim!) && o.Id == opbl.Id);

                if (existingOpBl == null)
                {
                    return false;
                }
                existingOpBl.Debit = opbl.Debit;
                existingOpBl.Credit = opbl.Credit;                
                existingOpBl.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                _context.OpeningBalances.Update(existingOpBl);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating branch: {ex.Message}");
                return false;
            }
        }

    }
}
