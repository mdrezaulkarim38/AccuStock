using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [StringLength(60)]
        public string? Name { get; set; }
        public int? CustomerType { get; set; }
        [StringLength(100)]
        public string? Address { get; set; }
        [StringLength(24)]
        public string? PhoneNumber { get; set; }
        [StringLength(50)]
        public string? Email { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public bool Status { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
