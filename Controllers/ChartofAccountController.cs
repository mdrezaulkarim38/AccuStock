using AccuStock.Interface;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers
{
    public class ChartofAccountController : Controller
    {
        private readonly IChartofAccount _chartofAccount;
        public ChartofAccountController(IChartofAccount chartofAccount)
        {
            _chartofAccount = chartofAccount;
        }
        [HttpGet]
        public async Task<IActionResult> ChartOfAccountList()
        {
            var ChartofAccountTypeList = await _chartofAccount.GetAllChartOfAccountType();
            ViewBag.ChartofAccountType = ChartofAccountTypeList;
            var ChartofAccountList = await _chartofAccount.GetAllChartOfAccount();
            return View(ChartofAccountList);
        }
    }
}
