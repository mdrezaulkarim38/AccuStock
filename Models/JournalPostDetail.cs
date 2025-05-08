namespace AccuStock.Models;

public class JournalPostDetail
{
    public int Id { get; set; }
    public int BusinessYearId { get; set; }
    public BusinessYear? BusinessYear { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int JournalPostId {  get; set; }
    public JournalPost? JournalPost { get; set; }
    public int ChartOfAccountId {  get; set; }
    public ChartOfAccount? ChartOfAccount { get; set; }
    public string? VchNo { get; set; }
    public DateTime? VchDate { get; set; }
    public int? VchType { get; set; }
    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public string? ChqNo {  get; set; }
    public DateTime? ChqDate {  get; set; }
    public string? Remarks {  get; set; }
    public int Status { get; set; } = 1; // 1 for pending, 2 =Approve and 3 = Reject;
    public int? PurchaseId { get; set; }
    public int? VendorPaymentId {  get; set; }
    public int? SaleId {  get; set; }
    public int? CustomerPaymentId {  get; set; }
    public string? Description {  get; set; }
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;
}
