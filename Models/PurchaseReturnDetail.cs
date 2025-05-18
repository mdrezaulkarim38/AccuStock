namespace AccuStock.Models
{
    public class PurchaseReturnDetail
    {
        public int Id { get; set; }
        public PurchaseReturn? PurchaseReturn { get; set; }
        public int PurchaseReturnId { get; set; }
        public PurchaseDetail? PurchaseDetail { get; set; }
        public int PurchaseDetailId { get; set; }
        public Product? Product { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Total { get; set; }        
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
