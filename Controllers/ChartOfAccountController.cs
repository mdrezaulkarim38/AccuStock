using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AccuStock.Controllers
{
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
            ViewBag.ChartofAccountType = await _chartOfAccount.GetAllChartOfAccountType();
            ViewBag.selectChartOfAccountList = await _chartOfAccount.GetAllChartOfAccount();
            return View(await _chartOfAccount.GetAllChartOfAccount());
        }


        [HttpGet]
        public async Task<IActionResult> GetChartOfAccountTypes()
        {
            var types = await _chartOfAccount.GetAllChartOfAccountType();
            return Json(types);
        }

        [HttpGet]
        public async Task<IActionResult> GetParentAccounts(int? chartOfAccountTypeId = null)
        {
            var accounts = await _chartOfAccount.GetAllChartOfAccount();
            if (chartOfAccountTypeId.HasValue)
            {
                accounts = accounts.Where(a => a.ChartOfAccountTypeId == chartOfAccountTypeId.Value).ToList();
            }
            return Json(accounts);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateChartOfAccount(ChartOfAccount model)
        {
            if (model.Id == 0)
            {
                bool isCreated = await _chartOfAccount.CreateChartOfAccount(model);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "Unsuccessfull to Create.";
                    return RedirectToAction("ChartOfAccountList");
                }

                TempData["SuccessMessage"] = "ChartOfAccount Year Created Successfully";
            }
            else
            {
                bool isUpdated = await _chartOfAccount.UpdateChartOfAccount(model);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Unsuccessfull to Create update";
                    return RedirectToAction("ChartOfAccountList");
                }

                TempData["SuccessMessage"] = "Chart Of Account Updated Successfully";
            }

            return RedirectToAction("ChartOfAccountList");
        }
    }
}
