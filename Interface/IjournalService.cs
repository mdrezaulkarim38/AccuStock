using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IJournalService
    {
        Task<List<JournalPost>> GetJournal();
        Task<bool> Createjournal(JournalPost journal);
        Task<bool> UpdateJournal(JournalPost journal);
    }
}
