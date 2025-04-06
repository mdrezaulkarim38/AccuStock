using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IBranchService _BranchService;
    private readonly IChartOfAccount _chartOfAccount;
    private readonly IGLedger _gLedgerService;

    public ReportsController(IBranchService BranchService, IChartOfAccount chartOfAccount, IGLedger gLedger)
    {
        _BranchService = BranchService;
        _chartOfAccount = chartOfAccount;
        _gLedgerService = gLedger;
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
    public IActionResult GetGlReport(JournalPost journalPost)
    {
        return View();
    }
}