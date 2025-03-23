using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.User
{
    public class CraftsmanRecommendationDto
    {
        [Required]
       public string CraftsmanFirstName { get; set; }
        [Required]

        public string CraftsmanLastName { get; set; }
        [Required]

        public string Governorate { get; set; }
        [Required]

        public string Location { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^01\d{9}$", ErrorMessage = "Invalid phone number format. It should start with 01 and have 11 digits.")]
        public string PhoneNumber { get; set; }
        [Required]
       public string Profession { get; set; }

        public string? PreviousWorkDescription { get; set; }
        public DateTime? DateTheProjectDone { get; set; }
        public IFormFile? PersonalPhoto { get; set; }
        public List<IFormFile>? PreviousWorkPictures { get; set; }
    }
}
