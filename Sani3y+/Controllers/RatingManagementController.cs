using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RatingManagementController : ControllerBase
    {
        private readonly IAdminRatingRepo _adminRatingRepo;

        public RatingManagementController(IAdminRatingRepo adminRepository)
        {
            _adminRatingRepo = adminRepository;
        }

        [HttpGet("all-ratings")]
        public async Task<IActionResult> GetAllRatings()
        {
            var ratings = await _adminRatingRepo.GetAllRatingsAsync();
            return Ok(ratings);
        }

        [HttpDelete("delete-rating/{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var success = await _adminRatingRepo.DeleteRatingAsync(id);
            return success ? Ok(new { message = "Rating deleted successfully." }) : NotFound(new { message = "Rating not found." });
        }
    }
}
