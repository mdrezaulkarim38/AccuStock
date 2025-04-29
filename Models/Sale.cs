namespace AccuStock.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int ProductId { get; set;}
        public Product? Product {get; set;}
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal SubAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? PaymentMethod { get; set; }
        public int? PaymentStatus { get; set; } = 1; // 1 = Unpaid, 2 = Paid
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public bool Status { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
