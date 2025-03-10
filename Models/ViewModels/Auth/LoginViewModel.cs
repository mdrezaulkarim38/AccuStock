using System.ComponentModel.DataAnnotations;
namespace AccuStock.Models.ViewModels.Auth;
public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}