using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using System.Security.Claims;

namespace AccuStock.Services
{
    public class BranchService : IBranchService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BranchService(AppDbContext context,
                            IWebHostEnvironment webHostEnvironment,
                            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }


        public async Task<bool> CreateBranch(Branch branch)
        {
            try
            {
                var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
                branch.SubscriptionId = int.Parse(subscriptionIdClaim!);
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.SubscriptionId == int.Parse(subscriptionIdClaim!));
                branch.CompanyId = company!.Id;

                await _context.Branches.AddAsync(branch);
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
                if (subscriptionIdClaim == null)
                {
                    return false;
                }

                var existingBranch = await _context.Branches
                    .FirstOrDefaultAsync(b => b.SubscriptionId == int.Parse(subscriptionIdClaim) && b.Id == branch.Id);

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
        public async Task<List<Branch>> GetAllBranches()
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId");

            if (subscriptionIdClaim == null || string.IsNullOrEmpty(subscriptionIdClaim.Value))
            {
                return new List<Branch>();
            }

            if (!int.TryParse(subscriptionIdClaim.Value, out int subscriptionId))
            {
                return new List<Branch>();
            }

            return await _context.Branches
                .Where(b => b.SubscriptionId == subscriptionId)
                .ToListAsync();
        }

    }
}