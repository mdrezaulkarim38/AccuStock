using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(150)]
        public string? CompanyName { get; set; }
        [StringLength(250)]
        public string? Address { get; set; }
        [StringLength(240)]
        public string? Contact { get; set; }
        [StringLength(60)]
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
