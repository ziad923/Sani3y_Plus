namespace Sani3y_.Dtos.Notifcation
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string CreatedAt { get; set; }  // Will store "منذ ساعة" format
        public bool IsRead { get; set; }
    }
}
