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

                // Check for duplicate category name under the same parent and subscription
                var exists = await _context.Categories
                    .AnyAsync(c => c.Name == category.Name 
                                && c.SubscriptionId == subscriptionIdClaim 
                                && c.ParentCategoryId == category.ParentCategoryId);
                if (exists)
                {
                    return false;
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var existingCat = await _context.Categories
                    .FirstOrDefaultAsync(b => b.Id == category.Id && b.SubscriptionId == subscriptionIdClaim);

                if (existingCat == null)
                {
                    return false;
                }

                var duplicateExists = await _context.Categories
                    .AnyAsync(c => c.Name == category.Name 
                                && c.SubscriptionId == subscriptionIdClaim 
                                && c.ParentCategoryId == category.ParentCategoryId 
                                && c.Id != category.Id);
                if (duplicateExists)
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
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> DeleteCategory(int catId)
        {
            var category = await _context.Categories.FindAsync(catId);
            if (category == null)
            {
                return "Category not found.";
            }

            // Check if the category has children
            var hasChildren = await _context.Categories.AnyAsync(c => c.ParentCategoryId == catId);
            if (hasChildren)
            {
                return "Cannot delete category with subcategories.";
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return "Category deleted successfully.";
        }

        public async Task<List<Category>> GetAllCategory()
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c => c.SubscriptionId == subscriptionIdClaim)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id && c.SubscriptionId == subscriptionIdClaim);
        }
    }
}