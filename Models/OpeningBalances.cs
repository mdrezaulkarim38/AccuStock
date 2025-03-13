namespace AccuStock.Models
{
    public class OpeningBalances
    {
        public int Id { get; set; }
        public decimal? OpnDebit { get; set; }
        public decimal? OpnCredit { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? ClsDebit { get; set; }
        public decimal? ClsCredit { get; set; }
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
