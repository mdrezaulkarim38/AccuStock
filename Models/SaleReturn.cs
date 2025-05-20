using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;
public class SaleReturn
{
    public int Id { get; set; }
        [StringLength(150)]
        public string? ReturnNo { get; set; }
        public int SaleId { get; set; }
        public Sale? Sale { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalAmount { get; set; }
        public int ReturnStatus { get; set; } = 0;
        [StringLength(100)]
        public string? Notes { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public List<SaleReturnDetail>? SaleReturnDetails { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
}
