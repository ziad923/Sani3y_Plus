namespace Sani3y_.Dtos.Craftman
{
    public class CompletedOrderDto
    {
        public string RequestNumber { get; set; }
        public string ClientFullName { get; set; }
        public string Location { get; set; }
        public string ServiceDescription { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}
