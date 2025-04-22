namespace AccuStock.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
