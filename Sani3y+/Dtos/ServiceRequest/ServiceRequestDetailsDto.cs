namespace Sani3y_.Dtos.ServiceRequest
{
    public class ServiceRequestDetailsDto
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public string ServiceDescription { get; set; }
        public string Address { get; set; }
        public DateTime StartDate { get; set; }
        public string PhoneNumber { get; set; }
        public string? SecondPhoneNumber { get; set; }
        public string? MalfunctionImagePath { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
    }
}
