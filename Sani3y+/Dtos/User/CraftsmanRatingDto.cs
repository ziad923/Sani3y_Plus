namespace Sani3y_.Dtos.User
{
    public class CraftsmanRatingDto
    {
        public string FullName { get; set; }
        public DateTime DateOfRate { get; set; }
        public int RatingByStars { get; set; }
        public string RatingDescription { get; set; }
    }
}
