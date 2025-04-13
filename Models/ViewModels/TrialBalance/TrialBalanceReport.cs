namespace AccuStock.Models.ViewModels.TrialBalance
{
    public class TrialBalanceReport
    {
        public string AccountName { get; set; } = string.Empty;
        public string AccountCode { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
