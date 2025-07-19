using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Repositry.Interfaces;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Craftsman")]
    public class CraftsmanOrdersController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly INotificationService _notificationService;
        public CraftsmanOrdersController(IServiceRequestService serviceRequestService,
                                        INotificationService notificationService)                      
        {
            _serviceRequestService = serviceRequestService;
            _notificationService = notificationService;
        }

        [HttpGet("craftsman/new-orders")]
        public async Task<IActionResult> GetNewOrders()
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(craftsmanId))
                return Unauthorized(new { Message = "Invalid craftsman ID." });

            var orders = await _serviceRequestService.GetNewOrdersAsync(craftsmanId);
            if (orders == null || !orders.Any())
                return NotFound(new { Message = "No new orders found." });

            return Ok(orders); 
        }

        [HttpGet("order-details/{requestNumber}")]
        public async Task<IActionResult> GetOrderDetails(string requestNumber)
        {
            var details = await _serviceRequestService.GetOrderDetailsAsync(requestNumber);
            if (details == null) return NotFound(new { Message = "Order not found." });
            return Ok(details);
        }

        [HttpGet("orders-under-implementation")]
        public async Task<IActionResult> GetUnderImplementationOrders()
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(craftsmanId)) return Unauthorized("Craftsman not authenticated.");

            var orders = await _serviceRequestService.GetUnderImplementationOrdersAsync(craftsmanId);
            return Ok(orders);
        }

        [HttpGet("orders-completed")]
        public async Task<IActionResult> GetCompletedOrders()
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(craftsmanId))
                return Unauthorized(new { Message = "Craftsman not authenticated." });

            var orders = await _serviceRequestService.GetCompletedOrdersAsync(craftsmanId);
            return Ok(orders);
        }

        [HttpPost("accept-order/{requestNumber}")]
        public async Task<IActionResult> AcceptOrder(string requestNumber)
        {
            var order = await _serviceRequestService.AcceptOrderWithInfoAsync(requestNumber);

            if (!order.Success)
                return NotFound(new { Message = "Order not found or could not be accepted." });

            string title = "تم قبول طلبك";
            string message = $"عميلنا العزيز،\nتم قبول طلبك رقم #{requestNumber} بواسطة الصنايعي: {order.CraftsmanName}.\n" +
                             "سيتواصل معك الصنايعي في أقرب وقت على رقم هاتفك لترتيب والبدء في المشروع.";

            await _notificationService.SendNotificationAsync(order.UserId!, title, message);
            return Ok(new { Message = "Order accepted successfully." });
        }

        [HttpDelete("reject-order/{requestNumber}")]
        public async Task<IActionResult> RejectOrder(string requestNumber)
        {
            var success = await _serviceRequestService.RejectOrderAsync(requestNumber);
            if (!success) return NotFound(new { Message = "Order not found or could not be rejected." });
            return Ok(new { Message = "Order rejected successfully." });
        }

    }
}
