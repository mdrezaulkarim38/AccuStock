using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomer();
        Task<bool> CreateCustomer(Customer customer);
        Task<bool> UpdateCustomer(Customer customer);
        Task<string> DeleteCustomer(int CustomerId);
    }
}
