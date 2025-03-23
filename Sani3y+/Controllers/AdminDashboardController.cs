using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly ICraftsmanRecommendationService _recommendationService;
        public AdminDashboardController(ICraftsmanRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }
        [HttpGet("pending-recommendations")]
        public async Task<IActionResult> GetPendingRecommendations()
        {
            var recommendations = await _recommendationService.GetAllPendingRecommendationsAsync();
            return Ok(recommendations);
        }
        [HttpGet("recommendation/{id}")]
        public async Task<IActionResult> GetRecommendationById(int id)
        {
            var recommendation = await _recommendationService.GetRecommendationByIdAsync(id);
            if (recommendation == null) return NotFound(new { message = "Recommendation not found." });

            return Ok(recommendation);
        }
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveRecommendation(int id)
        {
            var recommendation = await _recommendationService.GetRecommendationByIdAsync(id);
            if (recommendation == null) return NotFound(new { message = "Recommendation not found." });

            await _recommendationService.ApproveRecommendationAsync(id);
            return Ok(new { message = "Recommendation approved successfully." });
        }
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectRecommendation(int id)
        {
            var recommendation = await _recommendationService.GetRecommendationByIdAsync(id);
            if (recommendation == null) return NotFound(new { message = "Recommendation not found." });

            await _recommendationService.RejectRecommendationAsync(id);
            return Ok(new { message = "Recommendation rejected successfully." });
        }
    }
}
