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
    public string? PaymentMethod { get; set; }
    public int PaymentStatus { get; set; } = 1; // 1 = Unpaid, 2 = Paid
    public decimal TotalAmount { get; set; } // Sum of all SaleDetails
    public ICollection<SaleDetails>? SaleDetails { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
}
