using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProfessionController : ControllerBase
    {
		private readonly IProfessionRepo _professionRepo;

		public ProfessionController(IProfessionRepo professionRepo)
		{
			_professionRepo = professionRepo;
		}
		[HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfessions()
        {
            var professions =   await  _professionRepo.GetAllAsync();
            return Ok(professions);
        }
        [HttpPost]
        public async Task<IActionResult> AddProfession([FromForm] ProfessionCreateOrUpdateDto dto)
        {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var profession = await _professionRepo.AddAsync(dto);
			return CreatedAtAction(nameof(GetProfessions), new { id = profession.Id }, profession);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfession(int id, [FromForm] ProfessionCreateOrUpdateDto dto)
        {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

            var isUpdated = await _professionRepo.UpdateAsync(id, dto);
            if (!isUpdated) 
                return NotFound();

			return Ok(new { Message = "Profession updated successfully." });
		}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfession(int id)
        {
            var isDeleted = await _professionRepo.DeleteAsync(id);
            if (!isDeleted) 
                return NotFound();

			return Ok(new { Message = "Profession deleted successfully." });
		}
    }
}
