using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.ServiceRequest
{
    public class ServiceRequestDto
    {
        [Required]
        public string CraftsmanId  { get; set; }
        [Required]
        public string ServiceDescription { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? SecondPhoneNumber { get; set; } // Optional
        public IFormFile? ImageFile { get; set; } // Optional Image
        
       // Craftsman to send the request to
    }
}
