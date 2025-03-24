using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers
{
    public class GlReport : Controller
    {
        private readonly IBranchService _BranchService;
        private readonly IChartOfAccount _chartOfAccount;

        public GlReport(IBranchService BranchService, IChartOfAccount chartOfAccount)
        {
            _BranchService = BranchService;
            _chartOfAccount = chartOfAccount;
        }

        [HttpGet]
        public async Task<IActionResult> GetGlReport()
        {
            var branches = await _BranchService.GetAllBranches();
            ViewBag.Branches = branches;
            var chartOfAccounts = await _chartOfAccount.GetAllChartOfAccount();
            ViewBag.ChartOfAccounts = chartOfAccounts;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetGlReport(JournalPost journalPost)
        {
            return View();
        }
    }
}
