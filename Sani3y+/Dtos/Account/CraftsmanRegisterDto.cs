using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.Account
{
    public class CraftsmanRegisterDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Governorate is required.")]
        public string Governorate { get; set; }
        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Profession is required.")]
        public string Profession { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^01\d{9}$", ErrorMessage = "Invalid phone number format. It should start with 01 and have 11 digits.")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
     
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public IFormFile CardImage { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
