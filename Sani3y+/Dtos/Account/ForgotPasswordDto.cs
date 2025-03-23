using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.Account
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
       // [Required]
      //  public string? ClientUrl { get; set; }
    }
}
