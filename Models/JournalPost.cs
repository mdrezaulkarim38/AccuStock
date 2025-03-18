namespace AccuStock.Models;

public class JournalPost
{
    public int Id { get; set; }
    public int BusinessYearId { get; set; }
    public BusinessYear? BusinessYear { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public string? VchNo { get; set; }
    public DateTime? VchDate { get; set; }

    public int? VchType {  get; set; }
    public decimal? Debit {  get; set; }
    public decimal? Credit {  get; set; }
    public bool? Status { get; set; }
    public int? PurchaseId { get; set; }
    public int? VendorPaymentId { get; set; }
    public int? SaleId {  get; set; }
    public int? CustomerPaymentId {  get; set; }
    public int? UserId {  get; set; }
    public User? User { get; set; }
    public string? RefNo { get; set; }
    public string? Notes { get; set; }
    public DateTime? Created { get; set; } = DateTime.Now;
    public DateTime? Updated { get; set; }
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
}
