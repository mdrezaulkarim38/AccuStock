using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        [StringLength(150)]
        public string? PurchaseNo { get; set; }
        public int VendorId { get; set; }
        public int BranchId { get; set; }
        public int PurchaseStatus { get; set; } // 0: Pending, 1: Completed, 2: Partial
        public DateTime PurchaseDate { get; set; }
        [StringLength(100)]
        public string? Notes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalAmount { get; set; }
        public int? PaymentMethod { get; set; } // 0: Cash (Immediate Payment), 1: Credit (Payable)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public Vendor? Vendor { get; set; } 
        public Branch? Branch { get; set; }
        public List<PurchaseDetail>? Details { get; set; }
    }
}
