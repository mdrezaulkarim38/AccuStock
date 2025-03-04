namespace AccuStock.Models;
public class Subscription
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}