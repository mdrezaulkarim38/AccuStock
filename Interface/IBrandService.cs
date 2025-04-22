using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IBrandService
    {
        Task<List<Category>> GetAllBrand();
        Task<bool> CreateBrand(Brand brand);
        Task<Category> GetBrandById(int id);
        Task<bool> UpdateBrand(Brand brand);
        Task<string> DeleteBrand(int brandId);
    }
}
