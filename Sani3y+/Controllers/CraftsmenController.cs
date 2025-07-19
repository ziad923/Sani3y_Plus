using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Helpers;
using Sani3y_.Hubs;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;
using Sani3y_.Services;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Craftsman")]
    public class CraftsmenController : ControllerBase 
    {
        private readonly ICraftsmanQueryRepo _craftsmanRepo;

        public CraftsmenController(ICraftsmanQueryRepo craftsmanRepo)
        {
            _craftsmanRepo = craftsmanRepo;
        }
        
        [HttpGet("statistics")]
        public async Task<IActionResult> GetDashboardStatistics()
        {

            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(craftsmanId))
            {
                return Unauthorized("Craftsman ID not found in the token.");
            }

            var statistics = await _craftsmanRepo.GetDashboardStatisticsAsync(craftsmanId);
            return Ok(statistics);
        }

        [HttpGet("ratings")]
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
      

    }
}
