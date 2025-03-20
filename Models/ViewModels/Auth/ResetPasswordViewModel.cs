using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models.ViewModels.Auth
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string? CurrentPassword { get; set; }

        [Required]       
        public string? NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
