using Microsoft.EntityFrameworkCore;
using Sani3y_.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string? CraftsmanId { get; set; }
        public AppUser Craftsman { get; set; }
        public string ServiceDescription { get; set; }
        public string Address { get; set; }
        public DateTime StartDate { get; set; } // When the work starts
        public string PhoneNumber { get; set; }
        public string? SecondPhoneNumber { get; set; } // Optional

        public string? MalfunctionImagePath { get; set; } // Optional image for damage

        //  Status: "Waiting", "Under Implementation", "Done"
        public OrderStatus Status { get; set; } = OrderStatus.WaitingForAcceptance;
        public string RequestNumber { get; set; } // Unique Request Number
        public DateTime? AcceptedDate { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedDate { get; set; }
        public int? RatingId { get; set; } // Nullable FK
        public Rating? Rating { get; set; } // Optional navigation property
    }
}
