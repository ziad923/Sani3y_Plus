namespace Sani3y_.Dtos.Craftman
{
    public class CraftsmanOrderDetailsDto
    {
        public string ServiceDescription { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public string PhoneNumber { get; set; }
        public string? SecondPhoneNumber { get; set; }
        public List<string>? MalfunctionPictures { get; set; }
        public string RequestStatus { get; set; }
    }
}
