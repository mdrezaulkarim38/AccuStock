using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategory();
        Task<bool> CreateCategory(Category category);
        Task<Category> GetCategoryById(int id);
        Task<bool> UpdateCategory(Category category);
        Task<String> DeleteCategory(int catId);
    }
}
