using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.Account
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        [Required]
        public string Token { get; set; } // The token from the reset link
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
