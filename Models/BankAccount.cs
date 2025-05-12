using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;

public class BankAccount
{
    public int Id { get; set; }
    [Required]
    [StringLength(60, MinimumLength = 3)]
    public string? BankName { get; set; }
    [StringLength(60)]
    public string? BranchName { get; set; }
    [StringLength(32)]
    public string? RoutingNo { get; set; }
    [StringLength(24)]
    public string? AccountNo { get; set; }
    [StringLength(150)]
    public string? Remarks { get; set; }
    public bool Status { get; set; } = true;
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }

}