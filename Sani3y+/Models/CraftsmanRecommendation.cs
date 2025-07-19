using Sani3y_.Enums;

namespace Sani3y_.Models
{
    public class CraftsmanRecommendation
    {
        public int Id { get; set; }
        public string CraftsmanFirstName { get; set; }
        public string CraftsmanLastName { get; set; }
        public string Governorate { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public int ProfessionId { get; set; }

        // Optional Fields
        public string? PreviousWorkDescription { get; set; }
        public DateTime? DateTheProjectDone { get; set; }

        // File Paths
        public string? PersonalPhotoPath { get; set; }
        public List<string>? PreviousWorkPicturePaths { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public Profession Profession { get; set; }
        public RecommendationStatus Status { get; set; } = RecommendationStatus.Pending;

    }
}
