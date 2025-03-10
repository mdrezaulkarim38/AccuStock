using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;
[Authorize]
public class BusinessYearController : Controller
{
    private readonly IBusinessYear _businessYearService;
    public BusinessYearController(IBusinessYear businessYear)
    {
        _businessYearService = businessYear;
    }

    [HttpGet]
    public async Task<IActionResult> BusinessYearList()
    {
        var businessYear = await _businessYearService.GetAllBusinessYear();
        return View(businessYear);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateBusinessYear(BusinessYear businessyear)
    {
        if (businessyear.Id == 0)
        {
            bool isCreated = await _businessYearService.CreateBusinessYear(businessyear);
            if (!isCreated)
            {
                TempData["ErrorMessage"] = "This Business Year is already exists for this SubscriptionId.";
                return RedirectToAction("BusinessYearList");
            }
            TempData["SuccessMessage"] = "Business Year Created Successfully";
        }
        else
        {
            bool isUpdated = await _businessYearService.UpdateBusinessYear(businessyear);
            if (!isUpdated)
            {
                TempData["ErrorMessage"] = "Business Year already exists or update failed";
                return RedirectToAction("BusinessYearList");
            }
            TempData["SuccessMessage"] = "BusinessYear Updated Successfully";
        }

        return RedirectToAction("BusinessYearList");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBusinessYearStatusAsync(int busineesyearId)
    {
        bool success = await _businessYearService.ToggleBusinessYearStatusAsync(busineesyearId);

        if (!success)
            return NotFound();

        return RedirectToAction("BusinessYearList");
    }
}