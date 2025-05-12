using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccuStock.Models;
public class Role
{
    public int Id { get; set; }
    [StringLength(60)]
    public string? Name { get; set; }
}