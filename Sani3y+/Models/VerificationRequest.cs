namespace Sani3y_.Models
{
    public class VerificationRequest
    {
        public int Id { get; set; }
        public string CraftsmanId { get; set; }
        public AppUser Craftsman { get; set; }

        public string? ProfileImagePath { get; set; }
        public string? CardImagePath { get; set; }

        public DateTime SubmittedAt { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
    }
}
