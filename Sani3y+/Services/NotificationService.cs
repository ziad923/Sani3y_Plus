using Google;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Notifcation;
using Sani3y_.Hubs;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext; // Inject SignalR Hub
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(AppDbContext context, IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }
        public async Task<List<NotificationResponseDto>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _context.Notifications
         .Where(n => n.UserId == userId)
         .OrderByDescending(n => n.CreatedAt)
         .ToListAsync();

            return notifications.Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                CreatedAt = GetRelativeTime(n.CreatedAt), // Convert datetime to "منذ ساعة"
                IsRead = n.IsRead
            }).ToList();
        }

       

        public async Task SendNotificationAsync(string userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            try
            {
                // ✅ Send real-time notification via SignalR
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", title, message);
            }
            catch (Exception ex)
            {
                // Log the error but don't stop execution
                _logger.LogError($"Failed to send SignalR notification: {ex.Message}");
            }
        }
        public async Task<bool> MarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }
        private string GetRelativeTime(DateTime createdAt)
        {
            var timePassed = DateTime.UtcNow - createdAt;

            if (timePassed.TotalMinutes < 1)
                return "الآن";
            if (timePassed.TotalMinutes < 60)
                return $"منذ {(int)timePassed.TotalMinutes} دقيقة";
            if (timePassed.TotalHours < 24)
                return $"منذ {(int)timePassed.TotalHours} ساعة";
            if (timePassed.TotalDays < 30)
                return $"منذ {(int)timePassed.TotalDays} يوم";
            if (timePassed.TotalDays < 365)
                return $"منذ {(int)(timePassed.TotalDays / 30)} شهر";

            return $"منذ {(int)(timePassed.TotalDays / 365)} سنة";
        }
    }
}
