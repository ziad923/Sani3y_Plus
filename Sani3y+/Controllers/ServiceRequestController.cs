using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.ServiceRequest;
using Sani3y_.Dtos.User;
using Sani3y_.Enums;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRequestController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserRepo _userRepo;

        public ServiceRequestController(AppDbContext context , IUserRepo userRepo)
        {
            _context = context;
            _userRepo = userRepo;
        }
        [HttpPost("new-request")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> CreateRequest( [FromForm] ServiceRequestDto requestDto)
        {
            if(!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User Not Authentcated");
            }
            // Create the service request
            var response = await _userRepo.CreateServiceRequestAsync(userId,  requestDto);

            return Ok(response); ;

        }

        [Authorize(Roles = "User,Admin,Craftsman")]
        [HttpGet("request/{requestCode}")]
        public async Task<IActionResult> GetServiceRequestByCode(string requestCode)
        {
            var request = await _context.ServiceRequests
                .Where(r => r.RequestNumber == requestCode)
                .Select(r => new
                {
                    r.Id,
                    r.RequestNumber,
                    r.ServiceDescription,
                    r.Address,
                    r.StartDate,
                    r.PhoneNumber,
                    r.SecondPhoneNumber,
                    r.MalfunctionImagePath,
                    r.RequestDate,
                    r.Status
                })
                .FirstOrDefaultAsync();

            if (request == null)
                return NotFound(new { message = "Request not found" });

            return Ok(request);
        }
        [HttpPut("edit/{requestId}")]
        public async Task<IActionResult> EditRequest(string requestId, [FromForm] ServiceRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userRepo.EditServiceRequestAsync(requestId, userId, dto);
            return result ? Ok("Service request updated successfully.") : BadRequest("Request not found or cannot be edited.");
        }
        [HttpDelete("cancel/{requestId}")]
        public async Task<IActionResult> CancelRequest(string requestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userRepo.CancelServiceRequestAsync(requestId, userId);
            return result ? Ok("Service request canceled successfully.") : BadRequest("Request not found or cannot be canceled.");
        }
        [HttpPut("complete/{requestId}")]
        public async Task<IActionResult> CompleteRequest(string requestId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _userRepo.MarkRequestAsCompleteAsync(requestId, userId);
            return result ? Ok("Request marked as complete.") : BadRequest("Request not found or cannot be completed.");
        }
        [HttpPost("rate/{requestId}")]
        public async Task<IActionResult> AddRating(string requestId, [FromBody] UserRatingDto ratingDto)
        {
            var result = await _userRepo.AddRatingAsync(requestId, ratingDto.Stars, ratingDto.Description);
            return result ? Ok("Rating added successfully.") : BadRequest("Unable to add rating. Ensure the request is completed.");
        }

    }
}

