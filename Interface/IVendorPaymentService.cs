using AccuStock.Models;
using AccuStock.Models.ViewModels.VendorPayment;

namespace AccuStock.Interface
{
    public interface IVendorPaymentService
    {
        Task<List<VendorPaymentViewModel>> GetVendorPurchasesAsync(int vendorId, DateTime startDate, DateTime endDate);
        Task<bool> MakePaymentAsync(VendorPaymentRequestVM request);
        Task<List<VendorPayment>> GetAllPayment();
    }
}
