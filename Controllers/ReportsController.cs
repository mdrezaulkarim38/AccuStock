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
    public async Task<IActionResult> GetGlReport(DateTime? startDate, DateTime? endDate, int? branchId, int? chartOfAccountId, string reportType)
    {
        // Validate the date range
        if (startDate == null || endDate == null)
        {
            ModelState.AddModelError(string.Empty, "Please select a valid date range.");
            return View();
        }

        // Get the data based on selected filters (you will need to implement the actual report fetching)
        var glEntries = await _gLedgerService.GetAllGLedger(); // Here you can pass your filters like startDate, endDate, branchId, and chartOfAccountId

        // Generate the report based on the report type (PDF or Excel)
        if (reportType == "PDF")
        {
            // Logic to generate the PDF
            // return new PdfReportResult(glEntries); // Replace with actual logic to generate PDF report
        }
        else if (reportType == "Excel")
        {
            // Logic to generate the Excel report
            //return new ExcelReportResult(glEntries); // Replace with actual logic to generate Excel report
        }

        return View(glEntries); // Return the data for rendering in the view
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

