using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.User;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="User")]
    public class ExploreProfessionsController : ControllerBase  
    {
        private readonly IProfessionRepo _professionRepo;

        public ExploreProfessionsController(IProfessionRepo professionRepo)
        {
           _professionRepo = professionRepo;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfessions()
        {
            var professions = await _professionRepo.GetProfessionsWithImagesAsync();
            return Ok(professions);
        }
    }
}
