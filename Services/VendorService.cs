using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class VendorService : IVendor
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;
        public VendorService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<List<Vendor>> GetAllVendor()
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            var vendors = await _context.Vendors.Where(c => c.SubscriptionId == subscriptionIdClaim).ToListAsync();
            return vendors;
        }

        public async Task<Vendor> GetVendorById(int id)
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionIdClaim && c.Id == id);

            return vendor!;
        }
        public async Task<bool> CreateVendor(Vendor vendor)
        {
            try
            {
                vendor.SubscriptionId = _baseService.GetSubscriptionId();

                int maxChartOfAccountId = await _context.ChartOfAccounts
                   .Where(c => c.SubscriptionId == vendor.SubscriptionId)
                   .MaxAsync(c => (int?)c.Id) ?? 0;
                maxChartOfAccountId += 1;

                var chartOfAccount = new ChartOfAccount
                {
                    Name = vendor.Name,
                    AccountCode = maxChartOfAccountId.ToString(),
                    ChartOfAccountTypeId = 14,
                    SubscriptionId = vendor.SubscriptionId,
                    UserId = _baseService.GetUserId()
                };

                _context.ChartOfAccounts.Add(chartOfAccount);
                await _context.SaveChangesAsync();

                vendor.ChartOfAccountId = chartOfAccount.Id;
                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateVendor(Vendor vendor)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var existingVendor = await _context.Vendors.FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionIdClaim && c.Id == vendor.Id);
                if (existingVendor == null)
                {
                    return false;
                }
                existingVendor.Name = vendor.Name;
                existingVendor.CompanyName = vendor.CompanyName;
                existingVendor.Address = vendor.Address;
                existingVendor.Contact = vendor.Contact;
                existingVendor.Email = vendor.Email;
                
                _context.Vendors.Update(existingVendor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<bool> DeleteVendor(int id)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var vendor = await _context.Vendors.FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionIdClaim && c.Id == id);
                if (vendor == null)
                {
                    return false;
                }
                _context.Vendors.Remove(vendor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<bool> ToggleVendorStatus(int id)
        {
            try
            {
                var vendor = await _context.Vendors.FindAsync(id);
                if (vendor == null)
                    return false;

                vendor.Status = !vendor.Status;
                _context.Vendors.Update(vendor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
