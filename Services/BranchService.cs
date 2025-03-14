using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;

namespace AccuStock.Services
{
    public class BranchService : IBranchService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BranchService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CreateBranch(Branch branch)
        {
            try
            {
                var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
                branch.SubscriptionId = int.Parse(subscriptionIdClaim!);
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.SubscriptionId == int.Parse(subscriptionIdClaim!));
                branch.CompanyId = company!.Id;

                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateBranch(Branch branch)
        {
            try
            {
                var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
                var existingBranch = await _context.Branches.FirstOrDefaultAsync(b => b.SubscriptionId == int.Parse(subscriptionIdClaim!) && b.Id == branch.Id);

                if (existingBranch == null)
                {
                    return false;
                }

                existingBranch.BranchType = branch.BranchType;
                existingBranch.Name = branch.Name;
                existingBranch.Contact = branch.Contact;
                existingBranch.Address = branch.Address;

                _context.Branches.Update(existingBranch);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating branch: {ex.Message}");
                return false;
            }
        }

        public async Task<string> DeleteBranch(int branchId)
        {
            var branch = await _context.Branches.FindAsync(branchId);
            if (branch == null)
                return "Branch not found.";

            bool hasUsers = await _context.Users.AnyAsync(u => u.BranchId == branchId);
            if (hasUsers)
                return "Cannot delete this branch because users are assigned to it.";

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return "Branch deleted successfully.";
        }

        public async Task<List<Branch>> GetAllBranches()
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            return await _context.Branches.Where(b => b.SubscriptionId == int.Parse(subscriptionIdClaim!)).ToListAsync();
        }


    }
}