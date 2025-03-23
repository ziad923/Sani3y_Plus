using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.User
{
    public class UserRatingDto
    {
        [Required]
        [Range(1, 5)]
        public int Stars { get; set; } 
        public string? Description { get; set; }

        //public string UserName { get; set; }
        //public string CraftsmanId { get; set; } // Add CraftsmanId here
        //public DateTime? CreatedAt { get; set;}
    }
}
