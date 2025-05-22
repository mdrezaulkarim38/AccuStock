namespace AccuStock.Models.ViewModels.Profit_Loss
{
    public class ProfitAndLossViewModel
    {
        public List<AccountAmountViewModel>? IncomeAccounts { get; set; }
        public List<AccountAmountViewModel>? ExpenseAccounts { get; set; }
        public decimal TotalIncome => IncomeAccounts?.Sum(a => a.Amount) ?? 0;
        public decimal TotalExpense => ExpenseAccounts?.Sum(a => a.Amount) ?? 0;
        public decimal NetProfitOrLoss => TotalIncome - TotalExpense;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
