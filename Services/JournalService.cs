using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AccuStock.Services
{
    public class JournalService : IJournalService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBusinessYear _businessYearService; // Add dependency

        public JournalService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IBusinessYear businessYearService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _businessYearService = businessYearService;
        }

        public async Task<bool> CreateJournal(JournalPost journal)
        {
            if (journal == null) throw new ArgumentNullException(nameof(journal));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Set journal properties
                journal.SubscriptionId = GetSubscriptionId();
                journal.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                journal.VchNo = await GenerateVchNoAsync(journal.SubscriptionId);
                journal.Debit = journal.JournalPostDetails?.Sum(d => d.Debit) ?? 0;
                journal.Credit = journal.JournalPostDetails?.Sum(d => d.Credit) ?? 0;
                journal.Created = DateTime.Now;

                // Ensure a valid BusinessYearId
                journal.BusinessYearId =  await GetOrCreateBusinessYearId(journal.SubscriptionId, int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!));

                // Get default branch if needed
                journal.BranchId = journal.BranchId > 0 ? journal.BranchId : 1; // Assuming 1 as default branch ID
                journal.Status = 1; // Pending status

                // Add journal details
                if (journal.JournalPostDetails != null)
                {
                    foreach (var detail in journal.JournalPostDetails)
                    {
                        detail.SubscriptionId = journal.SubscriptionId;
                        detail.VchNo = journal.VchNo;
                        detail.VchDate = journal.VchDate;
                        detail.VchType = journal.VchType;
                        detail.BranchId = journal.BranchId;
                        detail.BusinessYearId = journal.BusinessYearId; // Set BusinessYearId for details
                        detail.Status = 1; // Pending status
                    }
                }

                await _context.JournalPosts.AddAsync(journal);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateJournal(JournalPost journal)
        {
            if (journal == null) throw new ArgumentNullException(nameof(journal));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int subscriptionId = GetSubscriptionId();
                var existingJournal = await _context.JournalPosts
                    .Include(j => j.JournalPostDetails)
                    .FirstOrDefaultAsync(j => j.SubscriptionId == subscriptionId && j.Id == journal.Id);

                if (existingJournal == null) return false;

                // Update journal properties
                existingJournal.UserId = int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                existingJournal.BranchId = journal.BranchId;
                existingJournal.VchNo = journal.VchNo;
                existingJournal.VchType = journal.VchType;
                existingJournal.VchDate = journal.VchDate;
                existingJournal.RefNo = journal.RefNo;
                existingJournal.Notes = journal.Notes;
                existingJournal.Debit = journal.JournalPostDetails?.Sum(d => d.Debit) ?? 0;
                existingJournal.Credit = journal.JournalPostDetails?.Sum(d => d.Credit) ?? 0;
                existingJournal.Updated = DateTime.Now;
                existingJournal.BusinessYearId = await GetOrCreateBusinessYearId(subscriptionId, int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!));

                // Update or add journal details
                if (existingJournal.JournalPostDetails != null)
                {
                    _context.JournalPostDetails.RemoveRange(existingJournal.JournalPostDetails);
                }

                if (journal.JournalPostDetails != null)
                {
                    foreach (var detail in journal.JournalPostDetails)
                    {
                        detail.SubscriptionId = journal.SubscriptionId;
                        detail.VchNo = journal.VchNo;
                        detail.VchDate = journal.VchDate;
                        detail.VchType = journal.VchType;
                        detail.BranchId = journal.BranchId;
                        detail.JournalPostId = existingJournal.Id;
                        detail.BusinessYearId = existingJournal.BusinessYearId;
                        detail.Status = 1;
                        _context.JournalPostDetails.Add(detail);
                    }
                }

                _context.JournalPosts.Update(existingJournal);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private async Task<int> GetOrCreateBusinessYearId(int subscriptionId, int userId)
        {
            int currentYear = DateTime.Now.Year;

            // Get all business years for the subscription
            var businessYears = await _businessYearService.GetAllBusinessYear();

            // Check if an active business year exists for the current year
            var businessYear = businessYears
                .FirstOrDefault(by => by.SubscriptionId == subscriptionId &&
                                     by.FromDate.Year <= currentYear &&
                                     by.ToDate.Year >= currentYear &&
                                     by.Status &&
                                     !by.ClosingStatus);

            if (businessYear == null)
            {
                // Create a new business year if none exists
                businessYear = new BusinessYear
                {
                    Name = $"Business Year {currentYear}",
                    FromDate = new DateTime(currentYear, 1, 1),
                    ToDate = new DateTime(currentYear, 12, 31),
                    Status = true,
                    ClosingStatus = false,
                    SubscriptionId = subscriptionId,
                    UserId = userId,
                    CreatedAt = DateTime.Now
                };
                await _businessYearService.CreateBusinessYear(businessYear);
            }

            return businessYear.Id;
        }

        private async Task<string> GenerateVchNoAsync(int subscriptionId)
        {
            int currentYear = DateTime.Now.Year;
            var lastVchNo = await _context.JournalPosts
                .Where(j => j.SubscriptionId == subscriptionId)
                .OrderByDescending(j => j.VchNo)
                .FirstOrDefaultAsync();

            if (lastVchNo == null)
            {
                return $"{currentYear}000001";
            }

            string lastVchNoString = lastVchNo.VchNo!.ToString();
            int lastYear = int.Parse(lastVchNoString.Substring(0, 4));
            int lastNumber = int.Parse(lastVchNoString.Substring(4));

            if (lastYear == currentYear)
            {
                lastNumber++;
            }
            else
            {
                lastNumber = 1;
            }

            return $"{currentYear}{lastNumber:D6}";
        }

        private int GetSubscriptionId()
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            return int.TryParse(subscriptionIdClaim, out var subscriptionId) ? subscriptionId : 0;
        }

        public async Task<List<JournalPost>> GetJournal()
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            return await _context.JournalPosts
                .Include(j => j.JournalPostDetails)
                .Where(b => b.SubscriptionId == int.Parse(subscriptionIdClaim!))
                .ToListAsync();
        }
    }
}