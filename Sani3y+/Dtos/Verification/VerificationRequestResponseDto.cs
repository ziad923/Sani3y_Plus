namespace Sani3y_.Dtos.Verification
{
    public class VerificationRequestResponseDto
    {
        public int Id { get; set; }
        public string CraftsmanId { get; set; }
        public string FullName { get; set; }
        public string ProfileImagePath { get; set; }
        public string CardImagePath { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
