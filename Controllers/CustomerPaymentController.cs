using AccuStock.Interface;
using AccuStock.Models.ViewModels.CustomerPayment;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AccuStock.Controllers
{
    public class CustomerPaymentController : Controller
    {
        private readonly ICustomerPaymentService _customerPaymentService;
        private readonly ICustomerService _customerService;
        public CustomerPaymentController(ICustomerPaymentService customerPaymentService, ICustomerService customer)
        {
            _customerPaymentService = customerPaymentService;
            _customerService = customer;
        }

        public async Task<IActionResult> CustomerPaymentList()
        {
            var payments = await _customerPaymentService.GetAllPayment();
            return View(payments);
        }

        [HttpGet]
        public async Task<IActionResult> ViewPayments(int customerId)
        {
            var customers = await _customerService.GetAllCustomer();
            ViewBag.CustomerList = new SelectList(customers, "Id", "Name", customerId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ViewPayments(int customerId, DateTime fromDate, DateTime toDate)
        {
            var data = await _customerPaymentService.GetCustomerSalesAsync(customerId, fromDate, toDate);
            var customers = await _customerService.GetAllCustomer();
            ViewBag.CustomerList = new SelectList(customers, "Id", "Name", customerId);
            ViewBag.FromDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.ToString("yyyy-MM-dd");
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment(CustomerPaymentRequestVM request, int customerId, DateTime fromDate, DateTime toDate)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data";
                return RedirectToAction("CustomerPaymentList");
            }
            try
            {
                await _customerPaymentService.MakePaymentAsync(request);
                TempData["Success"] = "Payment successful!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            // Reload updated list
            return RedirectToAction("CustomerPaymentList", new { customerId = customerId, fromDate = fromDate.ToString("yyyy-MM-dd"), toDate = toDate.ToString("yyyy-MM-dd") });
        }
    }
}
