using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;

        public CustomerService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<List<Customer>> GetAllCustomer()
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            var customers = await _context.Customers.Where(c => c.SubscriptionId == subscriptionIdClaim).ToListAsync();
            return customers;
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionIdClaim && c.Id == id);

            return customer!;
        }
        public async Task<bool> CreateCustomer(Customer customer)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                customer.SubscriptionId = subscriptionIdClaim;
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionIdClaim && c.Id == customer.Id);
                if (existingCustomer == null)
                {
                    return false;
                }
                existingCustomer.Name = customer.Name;
                existingCustomer.CustomerType = customer.CustomerType;
                existingCustomer.Address = customer.Address;
                existingCustomer.PhoneNumber = customer.PhoneNumber;
                existingCustomer.Email = customer.Email;
                _context.Customers.Update(existingCustomer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<bool> DeleteCustomer(int customerId)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionIdClaim && c.Id == customerId);
                if (customer == null)
                {
                    return false;
                }
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<bool> ToggleCustomerStatus(int CustomerId)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(CustomerId);
                if (customer == null)
                    return false;

                customer.Status = !customer.Status;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}