namespace Sani3y_.Dtos
{
    public class ContactUsResponseDto
    {
        public string RequestNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MessageContent { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? UserId { get; set; }
    }
}
