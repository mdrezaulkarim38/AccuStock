namespace AccuStock.Models
{
    public class OpeningBalances
    {
        public int Id { get; set; }
        public decimal? OpnDebit { get; set; } = 0.00M;
        public decimal? OpnCredit { get; set; } = 0.00M;
        public decimal? Debit { get; set; } = 0.00M;
        public decimal? Credit { get; set; } = 0.00M;
        public decimal? ClsDebit { get; set; } = 0.00M;
        public decimal? ClsCredit { get; set; } = 0.00M;
        public bool Status { get;set; } = true;
        public int ChartOfAccountId { get; set; }
        public ChartOfAccount? ChartOfAccount { get; set; }
        public int BusinessYearId { get; set; }
        public BusinessYear? BusinessYear { get; set; }
        public int SubScriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt {get; set; }
    }
}
