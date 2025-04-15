using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AccuStock.Controllers;
[Authorize]
public class ReportsController : Controller
{
    private readonly IBranchService _BranchService;
    private readonly IChartOfAccount _chartOfAccount;
    private readonly IGLedger _gLedgerService;  
    private readonly ITransactionService _transactionService;
    private readonly ITrialBalanceService _trialbalanceService;
    private readonly IProfitAndLossService _profitAndLossService;

    public ReportsController(IBranchService BranchService, IChartOfAccount chartOfAccount, IGLedger gLedger, ITransactionService transactionService, ITrialBalanceService trialBalanceService, IProfitAndLossService profitAndLossService)
    {
        _BranchService = BranchService;
        _chartOfAccount = chartOfAccount;
        _gLedgerService = gLedger;
        _transactionService = transactionService;
        _trialbalanceService = trialBalanceService;
        _profitAndLossService = profitAndLossService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGlReport()
    {
        var branches = await _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        var chartOfAccounts = await _chartOfAccount.GetAllChartOfAccount();
        ViewBag.ChartOfAccounts = chartOfAccounts;
        var getData = await _gLedgerService.GetAGLedgersList();
        return View(getData);
    }

    [HttpPost]
    public async Task<IActionResult> GetGlReport(DateTime? startDate, DateTime? endDate, int? branchId, int? chartOfAccountId, string reportType)
    {
        if (startDate == null || endDate == null)
        {
            ModelState.AddModelError(string.Empty, "Please select a valid date range.");
            return View();
        }

        var glEntries = await _gLedgerService.GetGLedger(startDate, endDate, branchId, chartOfAccountId);
        if (reportType == "PDF")
        {
        }
        else if (reportType == "Excel")
        {
        }
        var branches = await _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        var chartOfAccounts = await _chartOfAccount.GetAllChartOfAccount();
        ViewBag.ChartOfAccounts = chartOfAccounts;
        return View(glEntries);
    }

    [HttpGet]
    public async Task<IActionResult> GetAlltransAction()
    {
        var branches = await _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        var chartOfAccounts = await _chartOfAccount.GetAllChartOfAccount();
        ViewBag.ChartOfAccounts = chartOfAccounts;
        //var allEntries = await _transactionService.GetAllTransaction();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetAlltransAction(DateTime? startDate, DateTime? endDate, int? branchId)
    {
        if (startDate == null || endDate == null)
        {
            ModelState.AddModelError(string.Empty, "Please select a valid date range.");
            return View();
        }

        var allEntries = await _transactionService.GetAllTransAction(startDate, endDate, branchId);
        var branches = await _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        return View(allEntries);
    }

    [HttpGet]
    public async Task<IActionResult> GetTrialBbalance()
    {
        var branches = await _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetTrialBbalance(DateTime? startDate, DateTime? endDate, int? branchId)
    {
        if (startDate == null || endDate == null)
        {
            ModelState.AddModelError(string.Empty, "Please select a valid date range.");
        }
        var result = await _trialbalanceService.GetTrialBalanceAsync(startDate, endDate, branchId);
        var branches = await _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        return View(result);
    }

    public IActionResult ProfitAndLoss()
    {
        var branches = _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProfitAndLoss(DateTime fromDate, DateTime toDate, int branchId)
    {
        var model = await _profitAndLossService.GetProfitLossAsync(fromDate, toDate, branchId);
        var branches = _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        return View(model);
    }
}

