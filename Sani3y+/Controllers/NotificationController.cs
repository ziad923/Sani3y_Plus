using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Data;
using Sani3y_.Dtos.Notifcation;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet("Notifications")]
        [Authorize(Roles = "User,Craftsman,Admin")]
        public async Task<ActionResult<List<NotificationResponseDto>>> GetUserNotifications()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User  not found" });
            }

            return await _notificationService.GetUserNotificationsAsync(userId);
        }

        
        [HttpPost("send-notifications")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequestDto request)
        {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			await _notificationService.SendNotificationAsync(request.UserId, request.Title, request.Message);
            return Ok(new { message = "Notification sent successfully!" });
        }
        [HttpPatch("{id}/read")]
        [Authorize(Roles = "User,Craftsman,Admin")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _notificationService.MarkAsReadAsync(id);
            return notification ? NoContent() : NotFound();
        }
    }
}
