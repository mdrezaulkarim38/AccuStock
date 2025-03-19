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

        public JournalService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Createjournal(JournalPost journal)
        {
            if (journal == null) throw new ArgumentNullException(nameof(journal));
            try
            {
                journal.SubscriptionId = GetSubscriptionId();
                journal.UserId =
                    int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                journal.VchNo = await GenerateVchNoAsync(journal.SubscriptionId);
                _context.Add(journal);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateJournal(JournalPost journal)
        {
            if (journal == null) throw new ArgumentNullException(nameof(journal));
            try
            {
                int subscriptionId = GetSubscriptionId();
                var existingJournal = await _context.JournalPosts
                    .FirstOrDefaultAsync(j => j.SubscriptionId == subscriptionId && j.Id == journal.Id);

                if (existingJournal == null) return false;

                existingJournal.UserId =
                    int.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                existingJournal.BranchId = journal.BranchId;
                existingJournal.VchNo = journal.VchNo;
                existingJournal.VchType = journal.VchType;
                existingJournal.VchDate = journal.VchDate;
                existingJournal.Debit = journal.Debit;
                existingJournal.Credit = journal.Credit;
                existingJournal.RefNo = journal.RefNo;
                existingJournal.Notes = journal.Notes;
                existingJournal.Updated = DateTime.Now;
                _context.JournalPosts.Update(existingJournal);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private async Task<string> GenerateVchNoAsync(int subscriptionId)
        {
            // Get the current year
            int currentYear = DateTime.Now.Year;

            // Get the highest VchNo for the given subscriptionId in the current year
            var lastVchNo = await _context.JournalPosts
                .Where(j => j.SubscriptionId == subscriptionId)
                .OrderByDescending(j => j.VchNo)
                .FirstOrDefaultAsync();

            // If there is no existing VchNo, start from the first number
            if (lastVchNo == null)
            {
                return $"{currentYear}000001"; // Example: "20250001"
            }

            // Extract the year and number from the last VchNo
            string lastVchNoString = lastVchNo.VchNo.ToString();
            int lastYear = int.Parse(lastVchNoString.Substring(0, 4)); // Get the first 4 digits as year
            int lastNumber = int.Parse(lastVchNoString.Substring(4)); // Get the rest as the number part

            // If the last VchNo year matches the current year, increment the number
            if (lastYear == currentYear)
            {
                lastNumber++;
            }
            else
            {
                // If the year doesn't match, reset the counter to 1 for the new year
                lastNumber = 1;
            }

            // Generate the new VchNo (e.g., "20250002")
            return $"{currentYear}{lastNumber:D6}"; // Format number with leading zeroes to 6 digits
        }


        private int GetSubscriptionId()
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            return int.TryParse(subscriptionIdClaim, out var subscriptionId) ? subscriptionId : 0;
        }

        public async Task<List<JournalPost>> GetJournal()
        {
            var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
            return await _context.JournalPosts.Where(b => b.SubscriptionId == int.Parse(subscriptionIdClaim!))
                .ToListAsync();
        }
    }
}
