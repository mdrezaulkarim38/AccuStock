namespace AccuStock.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public string? Contact { get; set; }
        public string? Email { get; set; }
        public bool Status { get; set; } = true;
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public int? ChartOfAccountId { get; set; }
        public ChartOfAccount? ChartOfAccount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
