using AccuStock.Models;
using AccuStock.Models.ViewModels.CustomerPayment;

namespace AccuStock.Interface
{
    public interface ICustomerPaymentService
    {
        Task<List<CustomerPaymentViewModel>> GetCustomerSalesAsync(int customerId, DateTime startDate, DateTime endDate);
        Task<bool> MakePaymentAsync(CustomerPaymentRequestVM request);
        Task<List<CustomerPayment>> GetAllPayment();
    }
}
