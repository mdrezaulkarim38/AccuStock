using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models
{
    public class PurchaseReturn
    {
        public int Id { get; set; }
        [StringLength(150)]
        public string? ReturnNo { get; set; }
        public int PurchaseId { get; set; }
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
        public int ReturnStatus { get; set; }
        [StringLength(100)]
        public string? Notes { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }               
        public List<PurchaseReturnDetail>? PurchaseReturnDetails { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
