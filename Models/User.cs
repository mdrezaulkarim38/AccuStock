namespace AccuStock.Models;

public class User
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public int? Mobile { get; set; }
    public string? Address { get; set; }
    public int? RoleId { get; set; }
    public Role? Role { get; set; }
    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
