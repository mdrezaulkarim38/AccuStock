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
        private readonly BaseService _baseService;
        private readonly IBusinessYear _businessYearService;

        public JournalService(AppDbContext context, BaseService baseService, IBusinessYear businessYearService)
        {
            _context = context;
            _baseService = baseService;
            _businessYearService = businessYearService;
        }

        public async Task<bool> CreateJournal(JournalPost journal)
        {
            if (journal == null) throw new ArgumentNullException(nameof(journal));

            int userId = _baseService.GetUserId();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                journal.SubscriptionId = _baseService.GetSubscriptionId();
                journal.UserId = userId;
                journal.VchNo = await GenerateVchNoAsync(journal.SubscriptionId);
                journal.Debit = journal.JournalPostDetails?.Sum(d => d.Debit) ?? 0;
                journal.Credit = journal.JournalPostDetails?.Sum(d => d.Credit) ?? 0;
                journal.Created = DateTime.Now;

                journal.BusinessYearId = await GetOrCreateBusinessYearId(journal.SubscriptionId, userId);

                journal.BranchId = journal.BranchId; 
                journal.Status = 1; 

                if (journal.JournalPostDetails != null)
                {
                    foreach (var detail in journal.JournalPostDetails)
                    {
                        detail.SubscriptionId = journal.SubscriptionId;
                        detail.VchNo = journal.VchNo;
                        detail.VchDate = journal.VchDate;
                        detail.VchType = journal.VchType;
                        detail.BranchId = journal.BranchId;
                        detail.BusinessYearId = journal.BusinessYearId;
                        detail.Status = 1; 
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

            int userId = _baseService.GetUserId();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int subscriptionId = _baseService.GetSubscriptionId();
                var existingJournal = await _context.JournalPosts
                    .Include(j => j.JournalPostDetails)
                    .FirstOrDefaultAsync(j => j.SubscriptionId == subscriptionId && j.Id == journal.Id);

                if (existingJournal == null) return false;
                existingJournal.UserId = userId;
                existingJournal.BranchId = journal.BranchId;
                existingJournal.VchNo = journal.VchNo;
                existingJournal.VchType = journal.VchType;
                existingJournal.VchDate = journal.VchDate;
                existingJournal.RefNo = journal.RefNo;
                existingJournal.Notes = journal.Notes;
                existingJournal.Debit = journal.JournalPostDetails?.Sum(d => d.Debit) ?? 0;
                existingJournal.Credit = journal.JournalPostDetails?.Sum(d => d.Credit) ?? 0;
                existingJournal.Updated = DateTime.Now;
                existingJournal.BusinessYearId = await GetOrCreateBusinessYearId(subscriptionId, userId);

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

            var businessYears = await _businessYearService.GetAllBusinessYear();
            var businessYear = businessYears
                .FirstOrDefault(by => by.SubscriptionId == subscriptionId &&
                                     by.FromDate.Year <= currentYear &&
                                     by.ToDate.Year >= currentYear &&
                                     by.Status &&
                                     !by.ClosingStatus);

            if (businessYear == null)
            {
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

        
        public async Task<List<JournalPost>> GetJournal()
        {
            return await _context.JournalPosts
                .Include(j => j.JournalPostDetails)
                .Where(b => b.SubscriptionId == _baseService.GetSubscriptionId())
                .ToListAsync();
        }
    }
}