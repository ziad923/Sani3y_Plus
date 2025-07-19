using System.ComponentModel.DataAnnotations.Schema;

namespace Sani3y_.Models
{
    public class ContactUs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MessageContent { get; set; }
        public string RequestNumber { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsResolved { get; set; } = false;
        public DateTime? ResolvedAt { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }
    }
}
