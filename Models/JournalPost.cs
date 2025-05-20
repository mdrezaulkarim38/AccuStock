using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;

public class JournalPost
{
    public int Id { get; set; }
    public int BusinessYearId { get; set; }
    public BusinessYear? BusinessYear { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int? PurchaseReturnId { get; set; }
    public PurchaseReturn? PurchaseReturn { get; set; }
    public int? SaleReturnId { get; set; }
    public SaleReturn? SaleReturn { get; set; }
    [StringLength(60)]
    public string? VchNo { get; set; }
    public DateTime? VchDate { get; set; }
    public int? VchType { get; set; }
    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public int Status { get; set; } = 1;
    public int? PurchaseId { get; set; }
    public int? VendorPaymentId { get; set; }
    public int? SaleId { get; set; }
    public int? CustomerPaymentId { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [StringLength(100)]
    public string? RefNo { get; set; }
    [StringLength(100)]
    public string? Notes { get; set; }
    public DateTime? Created { get; set; } = DateTime.Now;
    public DateTime? Updated { get; set; } = DateTime.Now;
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    public List<JournalPostDetail> JournalPostDetails { get; set; } = new List<JournalPostDetail>();
}