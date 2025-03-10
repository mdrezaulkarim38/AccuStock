using AccuStock.Interface;
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
    public IActionResult BusinessYearList()
    {
        return View();
    }
}