using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.User
{
    public class UpdateProfileDto
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? PhoneNumber {  get; set; }

        public IFormFile ProfileImage { get; set; }
    }
}
