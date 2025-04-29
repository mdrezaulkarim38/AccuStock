using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomer();
        Task<Customer> GetCustomerById(int id);
        Task<bool> CreateCustomer(Customer customer);
        Task<bool> UpdateCustomer(Customer customer);
        Task<bool> DeleteCustomer(int CustomerId);
        Task<bool> ToggleCustomerStatus(int CustomerId);
    }
}
