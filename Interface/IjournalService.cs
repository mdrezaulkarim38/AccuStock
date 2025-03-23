using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IJournalService
    {
        Task<List<JournalPost>> GetJournal();
        Task<bool> CreateJournal(JournalPost journal);
        Task<bool> UpdateJournal(JournalPost journal);
    }
}