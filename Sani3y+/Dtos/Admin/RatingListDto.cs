namespace Sani3y_.Dtos.Admin
{
    public class RatingListDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string CraftsmanId { get; set; }
        public string CraftsmanFullName { get; set; }
        public int Stars { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public int ServiceRequestId { get; set; }
    }
}
