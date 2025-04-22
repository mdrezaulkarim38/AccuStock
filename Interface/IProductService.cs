using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProduct();
        Task<Product> GetProductbyId(int id);
        Task<bool> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<string> DeleteProduct(int productId);
    }
}
