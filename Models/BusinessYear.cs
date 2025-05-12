using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;

public class BusinessYear
{
    public int Id { get; set; }
    [StringLength(15)]
    public string? Name { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public bool Status { get; set; } = true;
    public bool ClosingStatus { get; set; } = false;
    public int UserId { get; set; }  
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
}
