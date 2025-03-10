using AccuStock.Interface;
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
    public async Task<IActionResult> ToggleBusinessYearStatusAsync(int userId)
    {
        bool success = await _businessYearService.ToggleBusinessYearStatusAsync(userId);

        if (!success)
            return NotFound();

        return RedirectToAction("BusinessYearList");
    }
}