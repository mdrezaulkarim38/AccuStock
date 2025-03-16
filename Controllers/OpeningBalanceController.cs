using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers
{
    public class OpeningBalanceController : Controller
    {
        private readonly IOpeningBalanceService _openingBalanceService;
        private readonly IChartOfAccount _chartOfAccountService;
        private readonly IBusinessYear _businessYear;
        public OpeningBalanceController(IOpeningBalanceService openingBalanceService, IChartOfAccount chartOfAccount, IBusinessYear businessYear)
        {
            _openingBalanceService = openingBalanceService;
            _chartOfAccountService = chartOfAccount;
            _businessYear = businessYear;
        }
        [HttpGet]
        public async Task<IActionResult> OpeningBalanceList()
        {               
            var chartOfAccount = await _chartOfAccountService.GetAllChartOfAccount();
            ViewBag.charofAccountsList = chartOfAccount;
            var opblList = await _openingBalanceService.GetOpBl();

            var businessYears = await _businessYear.GetAllBusinessYear();
            ViewBag.BusinessYears = businessYears;

            return View(opblList);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateOpeningBalance(OpeningBalances opbl)
        {           
            var accountType = Request.Form["AccountType"].ToString();
            var amount = Convert.ToDecimal(Request.Form["Amount"]);

            if (opbl.Id == 0)
            {
                if (accountType == "Debit")
                {
                    opbl.Debit = amount;
                    opbl.Credit = 0;
                }
                else if (accountType == "Credit")
                {
                    opbl.Credit = amount;
                    opbl.Debit = 0;
                }

                bool isCreated = await _openingBalanceService.CreateOpBl(opbl);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "An Error Occour For Creating Bpening Balance";
                    return RedirectToAction("OpeningBalanceList");
                }
                TempData["SuccessMessage"] = "Opening Balance Created Successfully";
            }
            else 
            {
                if (accountType == "Debit")
                {
                    opbl.Debit = amount;
                    opbl.Credit = 0;
                }
                else if (accountType == "Credit")
                {
                    opbl.Credit = amount;
                    opbl.Debit = 0;
                }

                bool isUpdated = await _openingBalanceService.UpdateOpBl(opbl);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "An Error Occour For Updating Opening Balance";
                    return RedirectToAction("Branch");
                }
                TempData["SuccessMessage"] = "Opening Balance Updated Successfully";
            }

            return RedirectToAction("OpeningBalanceList");
        }

    }
}
