using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AccuStock.Controllers;
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Customer()
        {
            var customers = await _customerService.GetAllCustomer();
            return View(customers);
        }
    [HttpGet]
    public async Task<IActionResult> AddOrEditCustomer(int id = 0)
    {
        if (id == 0)
        {
            return View(new Customer());
        }
        else
        {
            var customer = await _customerService.GetCustomerById(id);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found!";
                return RedirectToAction("Customer");
            }
            return View(customer);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddOrEditCustomer(Customer customer)
    {
        if (customer.Id == 0)
        {
            bool isCreated = await _customerService.CreateCustomer(customer);
            if (!isCreated)
            {
                TempData["ErrorMessage"] = "A customer already exists for this SubscriptionId.";
                return RedirectToAction("Customer");
            }
            TempData["SuccessMessage"] = "Customer Created Successfully";
        }
        else
        {
            bool isUpdated = await _customerService.UpdateCustomer(customer);
            if (!isUpdated)
            {
                TempData["ErrorMessage"] = "Customer name already exists or update failed";
                return RedirectToAction("Customer");
            }
            TempData["SuccessMessage"] = "Customer Updated Successfully";
        }
        return RedirectToAction("Customer");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int CustomerId)
    {
        bool success = await _customerService.ToggleCustomerStatus(CustomerId);

        if (!success)
        {
            TempData["ErrorMessage"] = "Customer Not Found";
            return RedirectToAction("Customer");
        }

        return RedirectToAction("Customer");
    }
    [HttpPost]

    public async Task<IActionResult> DeleteCus(int id)
    {
        var result = await _customerService.DeleteCustomer(id);
        if (!result)
        {
            TempData["ErrorMessageSweet"] = "Customer not Deleted";
        }
        else
        {
            TempData["SuccessMessageSweet"] = "Successfully Deleted";
        }
        return RedirectToAction("Customer");
    }
}
