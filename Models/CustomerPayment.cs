namespace AccuStock.Models
{
    public class CustomerPayment
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public Sale? Sale { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public int? PaymentMethod { get; set; } // 0 = Cash, 1 = Bank, etc.
        public string? Notes { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
