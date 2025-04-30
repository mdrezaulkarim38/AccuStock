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
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                vendor.SubscriptionId = subscriptionIdClaim;
                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
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
