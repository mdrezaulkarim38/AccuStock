using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IpurchaseReturnService
    {
        public Task<bool> CreatePurchaseReturn(PurchaseReturn purchaseReturn);
    }
}
