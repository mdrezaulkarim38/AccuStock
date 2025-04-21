using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;

        public CategoryService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<bool> CreateCategory(Category category)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                category.SubscriptionId = subscriptionIdClaim;
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var existingCat = await _context.Categories.FirstOrDefaultAsync(b => b.SubscriptionId == subscriptionIdClaim);

                if (existingCat == null)
                {
                    return false;
                }

                existingCat.Name = category.Name;
                existingCat.ParentCategoryId = category.ParentCategoryId;                
                existingCat.UpdatedAt = DateTime.Now;

                _context.Categories.Update(existingCat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<string> DeleteCategory(int catId)
        {
            var category = await _context.Categories.FindAsync(catId);
            if (category == null)
            {
                return "Category not found.";
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return "Category deleted successfully.";
        }
        public async Task<List<Category>> GetAllCategory()
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            var categories = await _context.Categories
                .Where(c => c.SubscriptionId == subscriptionIdClaim)
                .ToListAsync();
            return categories;
        }
    }
}
