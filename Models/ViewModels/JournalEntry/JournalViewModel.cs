namespace AccuStock.Models.ViewModels.JournalEntry;

public class JournalViewModel
{
    public ChartOfAccount? chartOfAccount{ get; set; }
    public ChartOfAccountType? chartOfAccountType{ get; set; }
    public JournalPost? journalPost{ get; set; }
    public JournalPostDetail? journalPostDetail{ get; set; }
}