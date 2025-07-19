namespace Sani3y_.Dtos.Model
{
    public class CraftsmanRecommendationDtoAi
    {
        public string Id { get; set; }
        public string ProfileImage { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string Governorate { get; set; }
        public string Location { get; set; }
        public double AverageRating { get; set; }
        public bool? isTrusted {  get; set; }
    }
}
