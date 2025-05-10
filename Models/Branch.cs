using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;
public class Branch
{
    public int Id { get; set; }
    public int BranchType { get; set; }
    [Required]
    [StringLength(50)]
    public string? Name { get; set; }
    [StringLength(90)]
    public string? Address { get; set; }
    [StringLength(20)]
    public string? Contact { get; set; }
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}