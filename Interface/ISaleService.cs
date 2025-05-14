using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface ISaleService
    {

        Task<List<Sale>> GetAllSale();
        Task<Sale?> GetSalebyId(int id);
        Task<int> GetSalebyInvNum(string invoiceNumber);
        Task<bool> CreateSale(Sale sale);
        Task<bool> UpdateSale(int saleId, Sale updatedSale);
        Task<string> DeleteSale(int saleId);
    }
}
