namespace AccuStock.Models
{
    public class BankAccounts
    {
        public int Id { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? RoutingNo { get; set; }
        public string? AccountNo { get; set; }
        public string? Remarks { get; set; }
        public bool Status { get; set; } = true;
        public int? UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }        
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }

    }
}
