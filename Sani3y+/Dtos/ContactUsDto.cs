using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos
{
    public class ContactUsDto
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required, MaxLength(1000)]
        public string MessageContent { get; set; }
    }
}
