using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models
{
    public class ProductStock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime Date { get; set; }
        public int QuantityIn { get; set; }
        public int QuantityOut { get; set; }
        [StringLength(90)]
        public string? ReferenceNo { get; set; }
        [StringLength(90)]
        public string? SourceType { get; set; } // e.g., "Purchase", "Sale", "Adjustment"
        public int SourceId { get; set; }
        [StringLength(100)]
        public string? Remarks { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
    }
}
