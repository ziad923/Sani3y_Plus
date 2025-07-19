using Sani3y_.Enums;

namespace Sani3y_.Dtos.Admin
{
    public class CraftsmanRecommendationResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CraftsmanFirstName { get; set; }
        public string CraftsmanLastName { get; set; }
        public string Governorate { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; } 
        public string ProfessionName { get; set; }
        public string? PreviousWorkDescription { get; set; }
        public DateTime? DateTheProjectDone { get; set; }
        public string? PersonalPhotoPath { get; set; }
        public List<string>? PreviousWorkPicturePaths { get; set; }
        public RecommendationStatus Status { get; set; }
    }
}
