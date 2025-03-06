using Microsoft.AspNetCore.Mvc;
using AccuStock.Interface;
using AccuStock.Models;

namespace AccuStock.Controllers;

public class SettingController : Controller
{
    private readonly ISettingService _SettingService;
    public SettingController(ISettingService SettingService)
    {
        _SettingService = SettingService;
    }
    [HttpGet]
    public async Task<IActionResult> Company()
    {
        var company = await _SettingService.GetCompanyBySubscriptionId();
        if (company == null)
        {
            return View(new Company());
        }
        return View(company);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Company(Company company)
    {
        if (!ModelState.IsValid)
        {
            return View(company);
        }

        try
        {
            if (company.Id == 0)
            {
                bool isCreated = await _SettingService.CreateCompanyAsync(company);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A company already exists for this SubscriptionId.";
                    return View(company);
                }
            }
            else
            {
                bool isUpdated = await _SettingService.UpdateCompanyAsync(company);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Company name already exists or update failed";
                    return View(company);
                }
            }
            TempData["SuccessMessage"] = "Company Created Successfully";
            return RedirectToAction("Company");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            return View(company);
        }
    }

    

    [HttpGet]
    public IActionResult UserList()
    {
        return View();
    }
}
