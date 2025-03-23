using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sani3y_.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to AspNetUsers (who rated)
        public string CraftsmanId { get; set; } // Craftsman being rated
        [Range(1, 5)]
        public int Stars { get; set; } // 1 to 5 stars only 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Description { get; set; }

        public AppUser User { get; set; }
        public AppUser Craftsman {  get; set; }
        public int ServiceRequestId { get; set; } // Foreign key in Rating
        public ServiceRequest ServiceRequest { get; set; }
    }
}
