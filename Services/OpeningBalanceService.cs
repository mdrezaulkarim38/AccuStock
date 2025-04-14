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
        private readonly BaseService _baseService;
        public OpeningBalanceService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<bool> CreateOpBl(OpeningBalances openingBalance)
        {
            try
            {
                openingBalance.SubScriptionId = _baseService.GetSubscriptionId();
                openingBalance.UserId = _baseService.GetUserId();
                
                await _context.OpeningBalances.AddAsync(openingBalance);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateOpBl(OpeningBalances opbl)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var existingOpBl = await _context.OpeningBalances.FirstOrDefaultAsync(o => o.SubScriptionId == subscriptionIdClaim && o.Id == opbl.Id);

                if (existingOpBl == null)
                {
                    return false;
                }
                existingOpBl.Debit = opbl.Debit;
                existingOpBl.Credit = opbl.Credit;                
                existingOpBl.UserId = _baseService.GetUserId();
                existingOpBl.UpdatedAt = DateTime.Now;
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
        public async Task<List<OpeningBalances>> GetOpBl()
        {
            return await _context.OpeningBalances.Where(o => o.SubScriptionId == _baseService.GetSubscriptionId()).ToListAsync();
        }
    }
}
