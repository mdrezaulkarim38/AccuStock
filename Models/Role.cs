using System.ComponentModel.DataAnnotations.Schema;

namespace AccuStock.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
