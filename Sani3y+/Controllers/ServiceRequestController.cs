using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Dtos.ServiceRequest;
using Sani3y_.Dtos.User;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="User")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IUserRatingRepository _userRatingRepository;
        private readonly INotificationService _notificationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestController(
            IUserRepo userRepo,
            IUserRatingRepository ratingRepository,
            INotificationService notificationService,
            UserManager<AppUser> userManager,
             IServiceRequestService serviceRequestService)
        {
            _userRepo = userRepo;
            _userRatingRepository = ratingRepository;
            _notificationService = notificationService;
            _userManager = userManager;
            _serviceRequestService = serviceRequestService;
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
            string title = "طلب جديد";
            string message = $"{requestDto.ServiceDescription}";

            await _notificationService.SendNotificationAsync(requestDto.CraftsmanId,  title, message);
            return Ok(response); ;

        }

        [Authorize(Roles = "User,Admin,Craftsman")]
        [HttpGet("request/{requestCode}")]
        public async Task<IActionResult> GetServiceRequestByCode(string requestCode) //
        {
            var request = await _serviceRequestService.GetRequestByCodeAsync(requestCode);

            if (request == null)
                return NotFound(new { message = "Request not found" });

            return Ok(request);
        }
        [HttpPut("edit/{requestId}")]
        public async Task<IActionResult> EditRequest(string requestId, [FromForm] ServiceRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
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
        [HttpGet("craftsman-info/{requestId}")]
        [Authorize(Roles = "User")] // or adjust roles as needed
        public async Task<IActionResult> GetCraftsmanInfo(string requestId)
        {
            var request = await _serviceRequestService.GetRequestWithCraftsmanAsync(requestId);
            if (request == null)
                return NotFound("Request not found.");

            var craftsman = request.Craftsman;

            var craftsmanInfo = new CraftsmanInfoDto
            {
                FullName = $"{craftsman.FirstName} {craftsman.LastName}",
                ProfilePicture = craftsman.ProfileImagePath 
            };

            return Ok(craftsmanInfo);
        }
        [HttpPost("rate/{requestId}")]
        public async Task<IActionResult> AddRating(string requestId, [FromBody] UserRatingDto ratingDto) // 
        {
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var request = await _serviceRequestService.GetRequestWithCraftsmanAsync(requestId);

            if (request == null)
                return BadRequest("Request not found.");
            var result = await _userRatingRepository.AddRatingAsync(requestId, ratingDto.Stars, ratingDto.Description);
            string title = "تقييم جديد";
            string message = $"قام {user.FirstName} {user.LastName} بتقييمك في المشروع الذي قمت به.";

            await _notificationService.SendNotificationAsync(request.CraftsmanId, title, message);
            return result ? Ok("Rating added successfully.") : BadRequest("Unable to add rating. Ensure the request is completed.");
        }

    }
}

