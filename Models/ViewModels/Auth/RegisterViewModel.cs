using System.ComponentModel.DataAnnotations;
namespace AccuStock.Models.ViewModels.Auth;
public class RegisterViewModel
{
    [Required]
    public string? FullName { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
    [Required]
    public int? ContactNumber { get; set; }
    public int? RoleId { get; set; } = 1;
}