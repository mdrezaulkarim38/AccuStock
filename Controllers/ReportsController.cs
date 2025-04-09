using AccuStock.Interface;
using AccuStock.Models;
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
    private readonly ICompanyService _companyService;  

    public ReportsController(IBranchService BranchService, IChartOfAccount chartOfAccount, IGLedger gLedger, ICompanyService companyService)
    {
        _BranchService = BranchService;
        _chartOfAccount = chartOfAccount;
        _gLedgerService = gLedger;
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGlReport()
    {
        var branches = await _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        var chartOfAccounts = await _chartOfAccount.GetAllChartOfAccount();
        ViewBag.ChartOfAccounts = chartOfAccounts;
        ViewBag.CompanyName = (await _companyService.GetCompanyBySubscriptionId())?.Name;
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
        ViewBag.CompanyName = (await _companyService.GetCompanyBySubscriptionId())?.Name;
        ViewBag.FromDate = startDate?.ToString("dd-MMM-yyyy");
        ViewBag.ToDate = endDate?.ToString("dd-MMM-yyyy");
        ViewBag.SelectedReportType = reportType;
        return View(glEntries);
    }

    public async Task<IActionResult> ATReport()
    {
        var branches = await _BranchService.GetAllBranches();
        ViewBag.Branches = branches;
        var chartOfAccounts = await _chartOfAccount.GetAllChartOfAccount();
        ViewBag.ChartOfAccounts = chartOfAccounts;
        return View();
    }
}

