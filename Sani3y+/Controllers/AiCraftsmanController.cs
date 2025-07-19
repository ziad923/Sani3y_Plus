using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Model;
using Sani3y_.Dtos.User;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User, Admin")]
    public class AiCraftsmanController : ControllerBase
    {
        private readonly IAiIntegrationService _aiService;
        private readonly IProfessionRepo _professionRepo;
        private readonly AppDbContext _context;

        public AiCraftsmanController(IAiIntegrationService aiService, AppDbContext context, IProfessionRepo  professionRepo)
        {
            _aiService = aiService;
            _context = context;
            _professionRepo = professionRepo;
        }
        [HttpGet("locations")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCraftsmanLocations()
        {
            var locations = await _context.Users
                .Where(u => u.Role == "Craftsman" && !string.IsNullOrEmpty(u.Location))
                .Select(u => u.Location)
                .Distinct()
                .ToListAsync();

            return Ok(new { locations });
        }
        [HttpPost("recommend")]
        [AllowAnonymous]
        public async Task<IActionResult> RecommendCraftsman([FromBody] AiQueryDto input, [FromQuery] string? location = null)
        {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (string.IsNullOrWhiteSpace(input.Query))
				return BadRequest("يرجي كتابة العطل لديك");

			try
            {

                var predictedProfession = await _aiService.GetPredictedProfessionAsync(input.Query);
                if (predictedProfession == null)
                {
                    
                    return BadRequest(new { message = "لم نتمكن من تحديد التخصص المناسب. يرجى المحاولة لاحقاً او التواصل معنا لحل مشكلتك." });
                }

                var profession = await _professionRepo.GetByNameAsync(predictedProfession);
                if (profession == null)
                {
                    return NotFound(new { message = $"نعتذر، لا يوجد صنّاع في مجال '{predictedProfession}' حالياً. نرجو التواصل معنا" });
                }
                var query = _context.Users
                             .Include(u => u.Ratings)
                             .Include(u => u.Profession)
                             .Where(u => u.Role == "Craftsman" && u.Profession.Name == predictedProfession);

                if (!string.IsNullOrWhiteSpace(location))
                {
                    query = query.Where(u => u.Location == location);
                }

                var craftsmen = await query
                            .Select(u => new CraftsmanRecommendationDtoAi
                            {
                                Id = u.Id,
                                ProfileImage = u.ProfileImagePath,
                                FullName = u.FirstName + " " + u.LastName,
                                Profession = u.Profession.Name,
                                Governorate = u.Governorate,
                                Location = u.Location,
                                AverageRating = u.Ratings.Any() ? u.Ratings.Average(r => r.Stars) : 0,
                                isTrusted = u.IsTrusted
                            })
                            .OrderByDescending(u => u.AverageRating)
                            .ToListAsync();

                if (craftsmen == null || craftsmen.Count == 0)
                {
                    
                    var existsInOtherLocations = await _context.Users
                        .AnyAsync(u => u.Role == "Craftsman" && u.Profession.Name == predictedProfession);

                    if (existsInOtherLocations)
                    {
                        return NotFound(new { message = $"لا يوجد صنّاع في تخصص '{predictedProfession}' في هذه المنطقة حالياً، لكن يوجد في مناطق أخرى." });
                    }
                    else
                    {
                        return NotFound(new { message = $"لا يوجد صنّاع حاليًا في تخصص '{predictedProfession}'. سيتم توفيرهم قريبًا إن شاء الله." });
                    }
                }


                return Ok(new { craftsmen });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة الطلب.", error = ex.Message });
            }
        }
    }
}
