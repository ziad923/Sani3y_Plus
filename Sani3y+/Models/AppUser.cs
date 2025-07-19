using Microsoft.AspNetCore.Identity;

namespace Sani3y_.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Governorate { get; set; } = string.Empty;
        public int? ProfessionId { get; set; }
        public string Location { get; set; } = string.Empty; // Only for craftsmen
        public string? CardImagePath { get; set; }  // Only for craftsmen
        public string? ProfileImagePath { get; set; } 
        public bool? IsTrusted { get; set; } // for craftman
        public string? GoogleId { get; set; }
        public string? RefreshToken {  get; set; }
        public DateTime RefreshTokenExpiryTime {  get; set; }
        // Navigation properties
        public Profession? Profession { get; set; }
        public List<ServiceRequest> Requests { get; set; } = new List<ServiceRequest>(); // User's requests
        public List<Rating> Ratings { get; set; } = new List<Rating>(); // User's ratings for craftsmen
        public List<PreviousWork> PreviousWorks { get; set; } = new List<PreviousWork>();
        public List<CraftsmanRecommendation> RecommendedCraftsmen { get; set; } = new List<CraftsmanRecommendation>();
    }
}
