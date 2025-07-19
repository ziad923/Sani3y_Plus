namespace Sani3y_.Dtos.Admin
{
    public class CraftsmanListDto : UserListDto
    {
        public string? CardImagePath { get; set; }
        public bool? IsTrusted { get; set; }
        public string? Profession { get; set; }
        public string Governorate { get; set; }
        public string Location { get; set; }
        public double AverageRating { get; set; }
    }
}
