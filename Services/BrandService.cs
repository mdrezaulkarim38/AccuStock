using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace AccuStock.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;
        public BrandService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<bool> CreateBrand(Brand brand)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                brand.SubscriptionId = subscriptionIdClaim;
                var existBrand = await _context.Brands.AnyAsync(b => b.Name == brand.Name && b.SubscriptionId == subscriptionIdClaim);
                if (existBrand)
                {
                    return false;
                }
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> UpdateBrand(Brand brand)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var existingBrand = await _context.Brands
                    .FirstOrDefaultAsync(b => b.Id == brand.Id && b.SubscriptionId == subscriptionIdClaim);
                if (existingBrand == null)
                {
                    return false;
                }
                existingBrand.Name = brand.Name;
                existingBrand.UpdatedAt = DateTime.Now;
                _context.Update(existingBrand);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<string> DeleteBrand(int brandId)
        {
            var brand = await _context.Brands.FindAsync(brandId);
            if(brand == null)
            {
                return "Brand not Found";
            }
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return "Brand Deleted Successfully";
        }
        public async Task<List<Brand>> GetAllBrand()
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            return await _context.Brands.Where(b => b.SubscriptionId == subscriptionIdClaim).ToListAsync();
        }
    }
}
