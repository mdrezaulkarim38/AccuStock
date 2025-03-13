using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers
{
    public class OpeningBalanceController : Controller
    {
        private readonly IOpeningBalanceService _openingBalanceService;
        private readonly IChartOfAccount _chartOfAccountService;
        public OpeningBalanceController(IOpeningBalanceService openingBalanceService, IChartOfAccount chartOfAccount)
        {
            _openingBalanceService = openingBalanceService;
            _chartOfAccountService = chartOfAccount;
        }
        [HttpGet]
        public async Task<IActionResult> OpeningBalanceList()
        {
            var chartOfAccount = await _chartOfAccountService.GetAllChartOfAccount();
             ViewBag.charofAccountsList = chartOfAccount;
            var opblList = await _openingBalanceService.GetOpBl();
            return View(opblList);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateOpeningBalance(OpeningBalances opbl)
        {
            if (opbl.Id == 0)
            {
                bool isCreated = await _openingBalanceService.CreateOpBl(opbl);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A Branch already exists for this SubscriptionId.";
                    return RedirectToAction("Branch");
                }
                TempData["SuccessMessage"] = "Branch Created Successfully";
            }
            else
            {
                bool isUpdated = await _openingBalanceService.UpdateOpBl(opbl);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Branch name already exists or update failed";
                    return RedirectToAction("Branch");
                }
                TempData["SuccessMessage"] = "Branch Updated Successfully";
            }

            return RedirectToAction("OpeningBalanceList");
        }
    }
}
