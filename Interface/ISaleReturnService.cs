using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface ISaleReturnService
    {
        Task<bool> CreateSaleReturn(SaleReturn saleReturn);
        Task<List<Sale>> GetSalesForReturn();
    }
}
