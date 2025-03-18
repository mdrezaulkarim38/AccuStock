using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IjournalService
    {
        Task<List<JournalPost>> GetJournal();
        Task<bool> Createjournal(JournalPost journal);
        Task<bool> UpdateOpBl(JournalPost journal);
    }
}
