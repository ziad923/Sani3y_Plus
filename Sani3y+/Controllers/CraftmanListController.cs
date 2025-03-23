using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Helpers;
using Sani3y_.Repositry.Interfaces;
using Sani3y_.Services;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CraftmanListController : ControllerBase // move the craftsman proflile logice in a serperate controoller
    {
        private readonly ICraftsmanRepo _craftsmanRepo;
        private readonly IServiceRequestService _serviceRequestService;

        public CraftmanListController(ICraftsmanRepo craftsmanRepo, IServiceRequestService serviceRequestService)
        {
            _craftsmanRepo = craftsmanRepo;
            _serviceRequestService = serviceRequestService;
        }
        
        [HttpGet("craftsmen")]
        [Authorize]
        public async Task<IActionResult> GetCraftsmen([FromQuery] QuereyObject query)
        {
            var (craftsmen, totalRecords) = await _craftsmanRepo.GetFilteredCraftsmenAsync(query);

            if (!craftsmen.Any())
            {
                return NotFound(new { Message = "No craftsmen found matching the criteria." });
            }

            return Ok(new
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,  // Total before pagination
                Data = craftsmen    // Only paginated results
            });
        }
        [HttpPost("upload-previous-work")]
        [Authorize(Roles = "Craftsman")]
        public async Task<IActionResult> UploadPreviousWork([FromForm] PreviousWorkDto previousWorkDto)
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(craftsmanId))
            {
                return Unauthorized(new { Message = "Craftsman not authenticated." });
            }

            await _craftsmanRepo.AddPreviousWorkAsync(craftsmanId, previousWorkDto);

            return Ok(new { Message = "Previous work uploaded successfully." });
        }
        [HttpGet("craftsman/new-orders")]
        [Authorize(Roles = "Craftsman")] // Ensure only craftsmen can access
        public async Task<IActionResult> GetNewOrders()
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(craftsmanId))
                return Unauthorized(new { Message = "Invalid craftsman ID." });

            var orders = await _serviceRequestService.GetNewOrdersAsync(craftsmanId);
            if (orders == null || !orders.Any())
                return NotFound(new { Message = "No new orders found." });

            return Ok(orders); // Return all new orders for the authenticated craftsman
        }
        [HttpGet("statistics")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            // 🎯 Retrieve the craftsmanId from the authenticated user's claims
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(craftsmanId))
            {
                return Unauthorized("Craftsman ID not found in the token.");
            }

            var statistics = await _craftsmanRepo.GetDashboardStatisticsAsync(craftsmanId);
            return Ok(statistics);
        }
        [HttpGet("order-details/{requestNumber}")]
        public async Task<IActionResult> GetOrderDetails(string requestNumber)
        {
            var details = await _serviceRequestService.GetOrderDetailsAsync(requestNumber);
            if (details == null) return NotFound(new { Message = "Order not found." });
            return Ok(details);
        }

        [HttpPost("accept-order/{requestNumber}")]
        public async Task<IActionResult> AcceptOrder(string requestNumber)
        {
            var success = await _serviceRequestService.AcceptOrderAsync(requestNumber);
            if (!success) return NotFound(new { Message = "Order not found or could not be accepted." });
            return Ok(new { Message = "Order accepted successfully." });
        }

        [HttpDelete("reject-order/{requestNumber}")]
        public async Task<IActionResult> RejectOrder(string requestNumber)
        {
            var success = await _serviceRequestService.RejectOrderAsync(requestNumber);
            if (!success) return NotFound(new { Message = "Order not found or could not be rejected." });
            return Ok(new { Message = "Order rejected successfully." });
        }
        [HttpGet("orders-completed")]
        public async Task<IActionResult> GetCompletedOrders()
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(craftsmanId))
                return Unauthorized("Craftsman not authenticated.");

            var orders = await _serviceRequestService.GetCompletedOrdersAsync(craftsmanId);
            return Ok(orders);
        }
        [HttpGet("orders-under-implementation")]
        public async Task<IActionResult> GetUnderImplementationOrders()
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(craftsmanId)) return Unauthorized("Craftsman not authenticated.");

            var orders = await _serviceRequestService.GetUnderImplementationOrdersAsync(craftsmanId);
            return Ok(orders);
        }


        [HttpGet("ratings")]
        [Authorize(Roles = "Craftsman")]
        public async Task<IActionResult> GetRatingsForCraftsman()
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(craftsmanId))
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }
            var ratings = await _craftsmanRepo.GetRatingsForCraftsmanAsync(craftsmanId);

            if (ratings == null || ratings.Count == 0)
            {
                return NotFound(new { Message = "No ratings found for this craftsman." });
            }

            return Ok(ratings);
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Craftsman")]
        public async Task<IActionResult> GetAllPreviousWorks()
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (craftsmanId == null) return Unauthorized("User ID not found.");

            var works = await _craftsmanRepo.GetPreviousWork(craftsmanId);
            return Ok(works);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Craftsman")]
        public async Task<IActionResult> UpdatePreviousWork(int id, [FromForm] PreviousWorkDto dto)
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (craftsmanId == null) return Unauthorized("User ID not found.");

            var success = await _craftsmanRepo.UpdatePreviousWorkAsync(id, dto, craftsmanId);
            return success ? Ok("Previous work updated successfully.") : NotFound("Previous work not found or unauthorized.");
        }
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Craftsman")]
        public async Task<IActionResult> DeletePreviousWork(int id)
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (craftsmanId == null) return Unauthorized("User ID not found.");

            var success = await _craftsmanRepo.DeletePreviousWorkAsync(id, craftsmanId);
            return success ? Ok("Previous work deleted successfully.") : NotFound("Previous work not found or unauthorized.");
        }

    }
}
