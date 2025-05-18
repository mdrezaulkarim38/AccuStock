using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models
{
    public class PurchaseReturn
    {
        public int Id { get; set; }
        [StringLength(150)]
        public string? ReturnNo { get; set; } // Unique identifier, e.g., "PR-001"
        public int PurchaseId { get; set; } // Links to original Purchase
        public Purchase? Purchase { get; set; }
        public int VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public DateTime ReturnDate { get; set; }
        [StringLength(100)]
        public string? Reason { get; set; } // e.g., "Defective", "Wrong Item"
        public decimal SubTotal { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalAmount { get; set; }
        public int ReturnStatus { get; set; } // 0: Pending, 1: Completed, 2: Cancelled
        [StringLength(100)]
        public string? Notes { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }               
        public List<PurchaseReturnDetail>? Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
