using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;
    public class VendorController : Controller
    {
        private readonly IVendor _vendorService;
        public VendorController(IVendor vendor)
        {
            _vendorService = vendor;
        }
    [HttpGet]
    public async Task<IActionResult> Vendor()
    {
        var vendorss = await _vendorService.GetAllVendor();
        return View(vendorss);
    }
    [HttpGet]
    public async Task<IActionResult> AddOrEditVendor(int id = 0)
    {
        if (id == 0)
        {
            return View(new Vendor());
        }
        else
        {
            var vendor = await _vendorService.GetVendorById(id);
            if (vendor== null)
            {
                TempData["ErrorMessage"] = "Vendor not found!";
                return RedirectToAction("Vendor");
            }
            return View(vendor);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddOrEditVendor(Vendor vendor)
    {
        if (vendor.Id == 0)
        {
            bool isCreated = await _vendorService.CreateVendor(vendor);
            if (!isCreated)
            {
                TempData["ErrorMessage"] = "Vendor already exists for this SubscriptionId.";
                return RedirectToAction("Vendor");
            }
            TempData["SuccessMessage"] = "Vendor Created Successfully";
        }
        else
        {
            bool isUpdated = await _vendorService.UpdateVendor(vendor);
            if (!isUpdated)
            {
                TempData["ErrorMessage"] = "Vendor name already exists or update failed";
                return RedirectToAction("Vendor");
            }
            TempData["SuccessMessage"] = "Vendor Updated Successfully";
        }
        return RedirectToAction("Vendor");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        bool success = await _vendorService.ToggleVendorStatus(id);

        if (!success)
        {
            TempData["ErrorMessage"] = "Vendor Not Found";
            return RedirectToAction("Vendor");
        }

        return RedirectToAction("Vendor");
    }
    [HttpPost]

    public async Task<IActionResult> DeleteVendor(int id)
    {
        var result = await _vendorService.DeleteVendor(id);
        if (!result)
        {
            TempData["ErrorMessageSweet"] = "Vendor not Deleted";
        }
        else
        {
            TempData["SuccessMessageSweet"] = "Successfully Deleted";
        }
        return RedirectToAction("Vendor");
    }
}
