using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Hangfire;
using AccuStock.Models;
using AccuStock.Models.ViewModels;
using AccuStock.Interface;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AccuStock.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IBranchService _BranchService;
        private readonly IChartOfAccount _chartOfAccount;
        private readonly IGLedger _gLedgerService;
        private readonly ITransactionService _transactionService;
        private readonly ITrialBalanceService _trialbalanceService;
        private readonly IProfitAndLossService _profitAndLossService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            IBranchService BranchService,
            IChartOfAccount chartOfAccount,
            IGLedger gLedger,
            ITransactionService transactionService,
            ITrialBalanceService trialBalanceService,
            IProfitAndLossService profitAndLossService,
            IHttpContextAccessor httpContextAccessor,
            IBackgroundJobClient backgroundJobClient,
            IEmailSender emailSender,
            ILogger<ReportsController> logger)
        {
            _BranchService = BranchService;
            _chartOfAccount = chartOfAccount;
            _gLedgerService = gLedger;
            _transactionService = transactionService;
            _trialbalanceService = trialBalanceService;
            _profitAndLossService = profitAndLossService;
            _httpContextAccessor = httpContextAccessor;
            _backgroundJobClient = backgroundJobClient;
            _emailSender = emailSender;
            _logger = logger;
        }

        // ... Other methods unchanged (GetGlReport, GetAlltransAction, GetTrialBbalance, ProfitAndLoss, SentReport) ...

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
                // TODO: Implement PDF generation
            }
            else if (reportType == "Excel")
            {
                // TODO: Implement Excel generation
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAlltransAction(DateTime? startDate, DateTime? endDate, int? branchId, int? chartOfAccountId)
        {
            if (startDate == null || endDate == null)
            {
                ModelState.AddModelError(string.Empty, "Please select a valid date range.");
                return View();
            }

            var allEntries = await _transactionService.GetAllTransAction(startDate, endDate, branchId, chartOfAccountId);
            var branches = await _BranchService.GetAllBranches();
            ViewBag.Branches = branches;
            var chartOfAccounts = await _chartOfAccount.GetAllChartOfAccount();
            ViewBag.ChartOfAccounts = chartOfAccounts;
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

        [HttpGet]
        public async Task<IActionResult> ProfitAndLoss()
        {
            var branches = await _BranchService.GetAllBranches();
            ViewBag.Branches = branches;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProfitAndLoss(DateTime fromDate, DateTime toDate, int branchId)
        {
            var model = await _profitAndLossService.GetProfitLossAsync(fromDate, toDate, branchId);
            var branches = await _BranchService.GetAllBranches();
            ViewBag.Branches = branches;
            return View(model);
        }

        [HttpGet]
        public IActionResult SentReport()
        {
            return View(new SentReportViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SentReport(SentReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                ModelState.AddModelError("", "Unable to retrieve user email.");
                return View(model);
            }

            // Parse the time to send
            if (!TimeSpan.TryParse(model.TimeToSend, out var timeToSend))
            {
                ModelState.AddModelError("TimeToSend", "Invalid time format.");
                return View(model);
            }

            // Validate date range
            if (model.FromDate == null || model.ToDate == null)
            {
                ModelState.AddModelError("", "Please select a valid date range.");
                return View(model);
            }

            if (model.FromDate > model.ToDate)
            {
                ModelState.AddModelError("FromDate", "From Date cannot be later than To Date.");
                return View(model);
            }

            // Calculate delay until the specified time today
            var now = DateTime.Now;
            var scheduledTime = new DateTime(now.Year, now.Month, now.Day, timeToSend.Hours, timeToSend.Minutes, 0);
            if (scheduledTime < now)
            {
                scheduledTime = scheduledTime.AddDays(1); // Schedule for tomorrow
            }
            var delay = scheduledTime - now;

            // Schedule the report generation job
            _backgroundJobClient.Schedule(() => GenerateAndSendReportAsync(
                new ReportRequest
                {
                    ReportType = model.ReportType,
                    TimeToSend = model.TimeToSend,
                    UserEmail = userEmail,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    SubscriptionId = int.Parse(subscriptionIdClaim!)
                }, null), delay);

            TempData["Message"] = $"Report scheduled to be sent at {model.TimeToSend} for date range {model.FromDate:yyyy-MM-dd} to {model.ToDate:yyyy-MM-dd}.";
            return RedirectToAction("SentReport");
        }

        [AutomaticRetry(Attempts = 5)]
        public async Task GenerateAndSendReportAsync(ReportRequest request, JobCancellationToken token)
        {
            _logger.LogInformation($"Generating report {request.ReportType} for {request.UserEmail}");
            string filePath = null;

            try
            {
                filePath = await GenerateReportAsync(request);

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogError($"Report file not found at: {filePath}");
                    throw new FileNotFoundException("Report file not found", filePath);
                }

                await _emailSender.SendEmailAsync(
                    request.UserEmail!,
                    $"{request.ReportType} Report",
                    "Please find the attached report.",
                    filePath);

                // Wait briefly to ensure file is released
                await Task.Delay(500);

                // Attempt to delete file with retry logic
                int retries = 3;
                while (retries > 0)
                {
                    try
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                            _logger.LogInformation($"Deleted file {filePath}");
                            break;
                        }
                        else
                        {
                            _logger.LogWarning($"File {filePath} does not exist for deletion");
                            break;
                        }
                    }
                    catch (IOException ex)
                    {
                        _logger.LogWarning(ex, $"Retrying file deletion for {filePath}. Attempts left: {retries}");
                        retries--;
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to delete file {filePath}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to generate/send report for {request.UserEmail}");
                throw; // Let Hangfire retry
            }
        }

        private async Task<string> GenerateReportAsync(ReportRequest request)
        {
            string fileName = $"Report_{request.ReportType}_{DateTime.Now:yyyyMMddHHmmss}.csv";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            try
            {
                using (var writer = new StreamWriter(filePath))
                {
                    // Use user-specified date range or default to last 100 days
                    var startDate = request.FromDate ?? DateTime.Now.AddDays(-100);
                    var endDate = request.ToDate ?? DateTime.Now;
                    var requestUserId = request.SubscriptionId;
                    _logger.LogInformation($"Generating {request.ReportType} report for date range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

                    if (request.ReportType == "GeneralLedger")
                    {
                        var glEntries = await _gLedgerService.GetGLedger(startDate, endDate, null, null, requestUserId);
                        _logger.LogInformation($"Retrieved {glEntries.Count} GeneralLedger entries");
                        await writer.WriteLineAsync("ChartOfAccountName,TotalDebit,TotalCredit");
                        foreach (var entry in glEntries)
                        {
                            await writer.WriteLineAsync($"\"{entry.ChartOfAccountName}\",{entry.TotalDebit},{entry.TotalCredit}");
                        }
                    }
                    else if (request.ReportType == "TrialBalance")
                    {
                        var trialBalance = await _trialbalanceService.GetTrialBalanceAsync(startDate, endDate, null);
                        await writer.WriteLineAsync("AccountName,Debit,Credit");
                        foreach (var entry in trialBalance)
                        {
                            await writer.WriteLineAsync($"\"{entry.AccountName}\",{entry.Debit},{entry.Credit}");
                        }
                    }
                    //else if (request.ReportType == "ProfitAndLoss")
                    //{
                    //    var profitLoss = await _profitAndLossService.GetProfitLossAsync(startDate, endDate, 0);
                    //    await writer.WriteLineAsync("Category,Amount");
                    //    foreach (var entry in profitLoss.Items)
                    //    {
                    //        await writer.WriteLineAsync($"\"{entry.Category}\",{entry.Amount}");
                    //    }
                    //}
                    else
                    {
                        await writer.WriteLineAsync("Sample Report Data");
                        await writer.WriteLineAsync($"Report Type: {request.ReportType}");
                        await writer.WriteLineAsync($"Scheduled Time: {request.TimeToSend}");
                    }
                }
                _logger.LogInformation($"Generated CSV file at {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to generate CSV file at {filePath}");
                throw;
            }
        }
    }
}