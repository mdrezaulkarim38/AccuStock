using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;
public class Sale
{
    public int Id { get; set; }
    [StringLength(60)]
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    [StringLength(60)]
    public int? PaymentMethod { get; set; } // 0: Credit Sale, 1: Cash Sale
    public int PaymentStatus { get; set; } = 0; // 0: Pending, 1: Completed, 2: Cancelled
    public decimal SubTotal { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<SaleDetails>? SaleDetails { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
}
