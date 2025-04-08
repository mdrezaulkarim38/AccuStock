namespace AccuStock.Models.ViewModels.GeneralLedger
{
    public class GLedger
    {
        public string? ChartOfAccountName { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Balance => TotalDebit - TotalCredit;
    }
}
