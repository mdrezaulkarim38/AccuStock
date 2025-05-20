using AccuStock.Models.ViewModels.Sale;

namespace AccuStock.Models
{
    public class SaleReturnDetail
    {
        public int Id { get; set; }
        public SaleReturn? SaleReturn { get; set; }
        public int SaleReturnId { get; set; }
        public SaleDetails? SaleDetail { get; set; }
        public int SaleDetailId { get; set; }
        public Product? Product { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Total { get; set; }
        public string? Reason { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
