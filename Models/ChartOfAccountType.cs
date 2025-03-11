namespace AccuStock.Models;
public class ChartOfAccountType
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int ParentId { get; set; } = 0;
    public int GroupID { get; set; } = 0;
}