using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAllBrand();
        Task<bool> CreateBrand(Brand brand);
        Task<bool> UpdateBrand(Brand brand);
        Task<string> DeleteBrand(int brandId);
    }
}
