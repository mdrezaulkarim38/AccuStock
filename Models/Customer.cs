namespace AccuStock.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? CustomerType { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public bool Status { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
