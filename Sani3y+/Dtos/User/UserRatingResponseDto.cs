namespace Sani3y_.Dtos.User
{
    public class UserRatingResponseDto
    {
        public string CraftsamanProfilePicture {  get; set; }
        public string CraftsmanFullName { get; set; }
        public string CraftsmanProfession { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Stars { get; set; }
        public string? Description { get; set; }
    }
}
