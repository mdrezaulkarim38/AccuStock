using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models.ViewModels.GeneralLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccuStock.Services
{
    public class GLedgerService : IGLedger
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;
        private readonly ILogger<GLedgerService> _logger;

        public GLedgerService(AppDbContext context, BaseService baseService, ILogger<GLedgerService> logger)
        {
            _context = context;
            _baseService = baseService;
            _logger = logger;
        }

        public async Task<List<GLedger>> GetGLedger(DateTime? startDate, DateTime? endDate, int? branchId, int? chartOfAccountId)
        {
            try
            {
                var subscriptionId = _baseService.GetSubscriptionId();
                _logger.LogInformation($"Fetching GLedger for SubscriptionId: {subscriptionId}, DateRange: {startDate} to {endDate}, BranchId: {branchId}, ChartOfAccountId: {chartOfAccountId}");

                var query = _context.JournalPostDetails
                    .Include(jpd => jpd.ChartOfAccount)
                    .Include(jpd => jpd.JournalPost)
                    .Where(jpd => jpd.SubscriptionId == subscriptionId)
                    .AsQueryable();

                if (startDate != null && endDate != null)
                {
                    query = query.Where(jpd => jpd.VchDate >= startDate && jpd.VchDate <= endDate);
                    _logger.LogInformation($"Applying date filter: {startDate} to {endDate}");
                }

                if (branchId != null)
                {
                    query = query.Where(jpd => jpd.JournalPost!.BranchId == branchId);
                }

                if (chartOfAccountId != null)
                {
                    query = query.Where(jpd => jpd.ChartOfAccountId == chartOfAccountId);
                }

                var groupData = await query
                    .GroupBy(jpd => jpd.ChartOfAccountId)
                    .Select(group => new GLedger
                    {
                        ChartOfAccountName = group.FirstOrDefault()!.ChartOfAccount != null
                            ? group.FirstOrDefault()!.ChartOfAccount!.Name
                            : "Unknown",
                        TotalDebit = group.Sum(jpd => jpd.Debit ?? 0),
                        TotalCredit = group.Sum(jpd => jpd.Credit ?? 0)
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {groupData.Count} GLedger entries");

                return groupData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch GLedger data");
                throw;
            }
        }

        public async Task<List<GLedger>> GetGLedger(DateTime? startDate, DateTime? endDate, int? branchId, int? chartOfAccountId, int? userReqestId)
        {
            try
            {
                var subscriptionId = _baseService.GetSubscriptionId();
                _logger.LogInformation($"Fetching GLedger for SubscriptionId: {subscriptionId}, DateRange: {startDate} to {endDate}, BranchId: {branchId}, ChartOfAccountId: {chartOfAccountId}");

                var query = _context.JournalPostDetails
                    .Include(jpd => jpd.ChartOfAccount)
                    .Include(jpd => jpd.JournalPost)
                    .Where(jpd => jpd.SubscriptionId == userReqestId)
                    .AsQueryable();

                if (startDate != null && endDate != null)
                {
                    query = query.Where(jpd => jpd.VchDate >= startDate && jpd.VchDate <= endDate);
                    _logger.LogInformation($"Applying date filter: {startDate} to {endDate}");
                }

                if (branchId != null)
                {
                    query = query.Where(jpd => jpd.JournalPost!.BranchId == branchId);
                }

                if (chartOfAccountId != null)
                {
                    query = query.Where(jpd => jpd.ChartOfAccountId == chartOfAccountId);
                }

                var groupData = await query
                    .GroupBy(jpd => jpd.ChartOfAccountId)
                    .Select(group => new GLedger
                    {
                        ChartOfAccountName = group.FirstOrDefault()!.ChartOfAccount != null
                            ? group.FirstOrDefault()!.ChartOfAccount.Name
                            : "Unknown",
                        TotalDebit = group.Sum(jpd => jpd.Debit ?? 0),
                        TotalCredit = group.Sum(jpd => jpd.Credit ?? 0)
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {groupData.Count} GLedger entries");

                return groupData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch GLedger data");
                throw;
            }
        }

        public async Task<List<GLedger>> GetAGLedgersList()
        {
            // Implement as needed, likely similar to GetGLedger without filters
            return await GetGLedger(null, null, null, null);
        }
    }
}