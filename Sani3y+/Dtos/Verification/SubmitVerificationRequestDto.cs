namespace Sani3y_.Dtos.Verification
{
    public class SubmitVerificationRequestDto
    {
        public IFormFile ProfileImage { get; set; }
        public IFormFile CardImage { get; set; }
    }
}
