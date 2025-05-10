using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccuStock.Models;

public class Product
{
    public int Id {get; set;}
    [StringLength(90)]
    public string? Name { get; set; }
    [StringLength(150)]
    public string? Description { get; set;}
    [StringLength(50)]
    public string? Code { get; set; }
    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public int? UnitId { get; set; }
    public Unit? Unit { get; set; }
    public string? ImagePath { get; set; }
    [NotMapped]
    public IFormFile? ProductImage { get; set; }
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
