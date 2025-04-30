using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IPurchaseService
    {
        Task<List<Purchase>> GetAllPurchase();
        Task<Purchase?> GetPurchasebyId(int id);
        Task<int> GetPurchasebyPurNum(string purchaseNum);
        Task<bool> CreatePurchase(Purchase purchase);
        Task<bool> UpdatePurchase(Purchase purchase);
        Task<string> DeletePurchase(int id);
    }
}
