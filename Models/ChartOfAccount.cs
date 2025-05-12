using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;
public class ChartOfAccount 
{
    public int Id { get; set;}
    [StringLength(60)]
    public string? Name { get; set;}
    public int? ParentId { get; set;}
    [StringLength(60)]
    public string? AccountCode { get; set;}
    public int ChartOfAccountTypeId { get; set;}
    public ChartOfAccountType? ChartOfAccountType{ get; set;}
    public int SubscriptionId { get; set;}
    public Subscription? Subscription{ get; set;}
    public int UserId { get; set; }
    public User? User{ get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set;}
}