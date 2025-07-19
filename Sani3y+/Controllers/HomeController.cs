using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetHomePageSummary()
        {
            var result = await _homeService.GetHomePageSummaryAsync();
            return Ok(result);
        }
        [HttpGet("system-stats")]
        public async Task<IActionResult> GetSystemStats()
        {
            var stats = await _homeService.GetSystemStatsAsync();
            return Ok(stats);
        }
    }
}
