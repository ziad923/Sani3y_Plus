using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Dtos.User;
using Sani3y_.Models;
using Sani3y_.Repositry;
using Sani3y_.Repositry.Interfaces;
using Sani3y_.Services;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICraftsmanRecommendationService _service;
        public UserController(IUserRepo userRepo, UserManager<AppUser> userManager, ICraftsmanRecommendationService service)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _service = service;
        }
        [HttpGet("profile")]
        [Authorize(Roles = "User,Craftsman")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Get the authenticated user's ID from the JWT token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Validate if userId is null or empty
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User is not authenticated." });
                }
                // Retrieve the user profile from the repository
                var profile = await _userRepo.GetUserProfileAsync(userId);

                // Check if profile exists
                if (profile == null)
                {
                    return NotFound(new { message = "User profile not found." });
                }

                // Return the profile data
                return Ok(profile);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new { message = "An error occurred while fetching the profile.", error = ex.Message });
            }
        }
        [HttpGet("GetAllUserRequests")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAllUserRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user ID from JWT

            var requests = await _userRepo.GetAllUserRequestsAsync(userId);
            if (requests == null || !requests.Any())
            {
                return NotFound("No service requests found for this user.");
            }

            return Ok(requests);
        }


        //[HttpPost("rate-craftsman")]
        //[Authorize(Roles = "User")]
        //public async Task<IActionResult> RateCraftsman([FromBody] UserRatingDto ratingDto, [FromQuery] string craftsmanId)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized(new { Message = "User not authenticated." });
        //    }

        //    // Add the rating
        //    var response = await _userRepo.AddRatingAsync(craftsmanId, userId, ratingDto);

        //    return Ok(response); ;
        //}
        [HttpGet("my-ratings")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserRatings()
        {
            try
            {
                // Get user ID from the token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "User not authenticated." });
                }

                // Fetch ratings from repository
                var ratings = await _userRepo.GetUserRatingsAsync(userId);

                // Check if there are no ratings
                if (ratings == null || !ratings.Any())
                {
                    return NotFound(new { Message = "No ratings found." });
                }

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                // Log the error and return a general error message
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }

        [HttpPut("update-profile")]
        [Authorize(Roles = "User, Craftsman")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            try
            {
                // Get the authenticated user's ID from the JWT token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Validate if userId is null or empty
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User is not authenticated." });
                }
                // Update the user profile
                var updatedProfile = await _userRepo.UpdateUserProfileAsync(userId, updateProfileDto);

                // Check if the profile was successfully updated
                if (updatedProfile == null)
                {
                    return NotFound(new { message = "User profile not found." });
                }

                return Ok(new { message = "User profile updated successfully." });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new { message = "An error occurred while updating the profile.", error = ex.Message });
            }
        }

        [HttpGet("craftsman/{craftsmanId}/previous-works")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> GetCraftsmanPreviousWorks(string craftsmanId)
        {
            var previousWorks = await _userRepo.GetCraftsmanPreviousWorkAsync(craftsmanId);

            if (previousWorks == null || !previousWorks.Any())
            {
                return NotFound("No previous work found for this craftsman.");
            }

            return Ok(previousWorks);
        }
        [HttpGet("{craftsmanId}/ratings")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> GetCraftsmanRatings(string craftsmanId)
        {
            var ratingsData = await _userRepo.GetCraftsmanRatingsAsync(craftsmanId);

            if (ratingsData.Ratings.Count == 0)
                return NotFound("No ratings found for this craftsman.");

            return Ok(ratingsData);
        }
        [HttpPost("recommendCraftsman")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RecommendCraftsman([FromForm] CraftsmanRecommendationDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _service.AddRecommendationAsync(model, user.Id);
            return Ok(new { message = "Craftsman recommendation sent to admin." });
        }
    }
}
