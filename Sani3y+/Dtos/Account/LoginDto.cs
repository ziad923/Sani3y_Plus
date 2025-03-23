using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.Account
{
    public class LoginDto
    {
        [Required]
        public string EmailOrPhone {  get; set; }
        [Required]
        public string Password { get; set; }
    }
}
