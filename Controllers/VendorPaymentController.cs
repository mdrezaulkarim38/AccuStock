using AccuStock.Interface;
using AccuStock.Models.ViewModels.VendorPayment;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace AccuStock.Controllers
{
    public class VendorPaymentController : Controller
    {
        private readonly IVendorPaymentService _vendorPaymentService;
        private readonly IVendor _vendorService;
        public VendorPaymentController(IVendorPaymentService vendorPaymentService, IVendor vendor)
        {
            _vendorPaymentService = vendorPaymentService;
            _vendorService = vendor;
        }
        public async Task<IActionResult> VendorPaymentList()
        {
            var payments = await _vendorPaymentService.GetAllPayment();
            return View(payments);
        }

        [HttpGet]
        public async Task<IActionResult> ViewPayments(int vendorId)
        {
            var Vendors = await _vendorService.GetAllVendor();
            ViewBag.VendorList = new SelectList(Vendors, "Id", "Name", vendorId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ViewPayments(int vendorId, DateTime fromDate, DateTime toDate)
        {
            var data = await _vendorPaymentService.GetVendorPurchasesAsync(vendorId, fromDate, toDate);
            var Vendors = await _vendorService.GetAllVendor();
            ViewBag.VendorList = new SelectList(Vendors, "Id", "Name", vendorId);
            ViewBag.FromDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.ToString("yyyy-MM-dd");
            return View(data);
        }
        // Handle payment post
        [HttpPost]        
        public async Task<IActionResult> MakePayment(VendorPaymentRequestVM request, int vendorId, DateTime fromDate, DateTime toDate)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data";
                return RedirectToAction("Index");
            }
            try
            {
                await _vendorPaymentService.MakePaymentAsync(request);
                TempData["Success"] = "Payment successful!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            // Reload updated list
            return RedirectToAction("Index", new { vendorId = vendorId, fromDate = fromDate.ToString("yyyy-MM-dd"), toDate = toDate.ToString("yyyy-MM-dd") });
        }
    }
}
