namespace Sani3y_.Dtos.ServiceRequest
{
    public class UpdateServiceRequestDto
    {
        public string ServiceDescription { get; set; }
        public string Address { get; set; }
        public DateTime StartDate { get; set; }
        public string PhoneNumber { get; set; }
        public string SecondPhoneNumber { get; set; }
        public IFormFile MalfunctionPicture { get; set; } // Optional
    }
}
