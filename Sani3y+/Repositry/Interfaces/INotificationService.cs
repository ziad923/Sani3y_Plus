using Sani3y_.Dtos.Notifcation;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationResponseDto>> GetUserNotificationsAsync(string userId);
        Task SendNotificationAsync(string userId, string title, string message);
        Task<bool> MarkAsReadAsync(int id);
    }
}
