using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;
public class ChartOfAccountType
{
    public int Id { get; set; }
    [StringLength(60)]
    public string? Name { get; set; }
    public int ParentId { get; set; } = 0;
    public int GroupId { get; set; } = 0;
}