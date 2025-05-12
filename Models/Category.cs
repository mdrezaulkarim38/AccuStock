using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccuStock.Models;
public class Category
{
    public int Id { get; set; }
    [StringLength(60)]
    public string? Name { get; set; }       
    public int? ParentCategoryId { get; set; }
    [ForeignKey("ParentCategoryId")]
    public Category? ParentCategory { get; set; }
    public int IsParent { get; set; } = 0;
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}