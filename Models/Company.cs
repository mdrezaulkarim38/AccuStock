using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccuStock.Models;
public class Company
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
    [StringLength(60)]
    public string? TagLine { get; set; }
    [StringLength(60)]
    public string? VatRegistrationNo { get; set; }
    [StringLength(60)]
    public string? TinNo { get; set; }
    [StringLength(60)]
    public string? WebsiteLink { get; set; }
    [StringLength(60)]
    public string? Email { get; set; }
    [StringLength(24)]
    public string? ContactNumber { get; set; }
    [StringLength(100)]
    public string? Address { get; set; }
    [StringLength(100)]
    public string? Remarks { get; set; }
    [StringLength(100)]
    public string? LogoPath { get; set; }
    [NotMapped]
    public IFormFile? LogoImage { get; set; }
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
