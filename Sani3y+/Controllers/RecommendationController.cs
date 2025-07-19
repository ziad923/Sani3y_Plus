using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Dtos.Account;
using Sani3y_.Enums;
using Sani3y_.Helpers;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;
using Sani3y_.Services;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class RecommendationController : ControllerBase
    {
        private readonly ICraftsmanRecommendationService _recommendationService;
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;
        private readonly UserManager<AppUser> _userManager;
        private readonly AvatarService _avatarService;

        public RecommendationController(
            ICraftsmanRecommendationService recommendationService,
            INotificationService notificationService,
            IFileService fileService,
            UserManager<AppUser> userManager,
            AvatarService avatarService)
        {
            _recommendationService = recommendationService;
            _notificationService = notificationService;
            _fileService = fileService;
            _userManager = userManager;
            _avatarService = avatarService;
        }

        [HttpGet("pending-recommendations")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingRecommendations()
        {
            var recommendations = await _recommendationService.GetAllPendingRecommendationsAsync();
            return Ok(recommendations);
        }
        [HttpGet("recommendation/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRecommendationById(int id)
        {
            var recommendation = await _recommendationService.GetRecommendationByIdAsync(id);
            if (recommendation == null) return NotFound(new { message = "Recommendation not found." });

            return Ok(recommendation);
        }
        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRecommendation(int id)
        {
            var recommendation = await _recommendationService.GetRecommendationByIdAsync(id);
            if (recommendation == null) return NotFound(new { message = "Recommendation not found." });

            await _recommendationService.ApproveRecommendationAsync(id);

            string title = "تمت الموافقة على توصيتك";
            string message = $"عميلنا العزيز،\nتمت الموافقة على توصيتك بخصوص {recommendation.CraftsmanFirstName} {recommendation.CraftsmanLastName}. نشكرك على التعاون معنا.";

            await _notificationService.SendNotificationAsync(recommendation.UserId, title, message);
            return Ok(new { message = "Recommendation approved successfully." });
        }
        [HttpPost("add-craftsman/{recommendationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCraftsman(int recommendationId, [FromForm] CraftsmanRegisterDto craftsmanRegister)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

              
                var recommendation = await _recommendationService.GetRecommendationByIdAsync(recommendationId);
                if (recommendation == null)
                    return NotFound(new { Message = "Recommendation not found." });

                if (recommendation.Status != RecommendationStatus.Approved)
                    return BadRequest(new { Message = "The recommendation has not been approved yet." });

                var existingUser = await _userManager.FindByEmailAsync(craftsmanRegister.Email);
                if (existingUser != null)
                    return BadRequest(new { Message = "Email already exists." });

                
                var cardImagePath = craftsmanRegister.CardImage != null ? await _fileService.SavePictureAsync(craftsmanRegister.CardImage) : null;

                var profileImagePath = craftsmanRegister.ProfileImage != null
                  ? await _fileService.SavePictureAsync(craftsmanRegister.ProfileImage)
                  : _avatarService.GetRandomAvatarPath();

                var appUser = new AppUser
                {
                    FirstName = craftsmanRegister.FirstName,
                    LastName = craftsmanRegister.LastName,
                    Governorate = craftsmanRegister.Governorate,
                    Location = craftsmanRegister.Location,
                    ProfessionId = craftsmanRegister.ProfessionId,
                    PhoneNumber = craftsmanRegister.PhoneNumber,
                    Email = craftsmanRegister.Email,
                    UserName = craftsmanRegister.Email,
                    CardImagePath = cardImagePath,
                    ProfileImagePath = profileImagePath,
                    IsTrusted = false,
                    Role = "Craftsman"
                };

                var createUser = await _userManager.CreateAsync(appUser, craftsmanRegister.Password);
                if (createUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "Craftsman");
                    if (roleResult.Succeeded)
                    {
                        return Ok(new { Message = "Craftsman User Created Successfully" });
                    }
                    else
                    {
                        return BadRequest(roleResult.Errors);
                    }
                }
                else
                {
                    return BadRequest(createUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }
        [HttpPost("reject/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectRecommendation(int id)
        {
            var recommendation = await _recommendationService.GetRecommendationByIdAsync(id);
            if (recommendation == null) return NotFound(new { message = "Recommendation not found." });

            await _recommendationService.RejectRecommendationAsync(id);
            return Ok(new { message = "Recommendation rejected successfully." });
        }
        [HttpGet("user-approved-recommendations")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserApprovedRecommendations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found.");

            var result = await _recommendationService.GetAllApprovedRecommendationsAsync(userId);
            return Ok(result);
        }
    }
}
