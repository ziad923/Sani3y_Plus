namespace Sani3y_.Dtos.User
{
    public class RecommendedCraftsmanDto
    {
        public string CraftsmanId { get; set; }
        public string ProfileImage { get; set; }
        public string CraftsmanFullName { get; set; }
        public string Profession {  get; set; }
        public string Governate {  get; set; }
        public string Location { get; set; }
        public double AvgRating {  get; set; }
        public bool isTrusted {  get; set; }
    }
}
