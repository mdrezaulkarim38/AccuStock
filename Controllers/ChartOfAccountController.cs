using AccuStock.Interface;
using AccuStock.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;

[Authorize]
public class ChartOfAccountController : Controller
{
    private readonly IChartOfAccount _chartOfAccount;
    public ChartOfAccountController(IChartOfAccount chartOfAccount)
    {
        _chartOfAccount = chartOfAccount;
    }
    [HttpGet]
    public async Task<IActionResult> ChartOfAccountList()
    {
        var ChartofAccountTypeList = await _chartOfAccount.GetAllChartOfAccountType();
        ViewBag.ChartofAccountType = ChartofAccountTypeList;
        var ChartofAccountList = await _chartOfAccount.GetAllChartOfAccount();
        return View(ChartofAccountList);
    }
}

