using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;
public class Unit
{
    public int Id { get; set; }
    [StringLength(60)]
    public string? Name { get; set; }
}

