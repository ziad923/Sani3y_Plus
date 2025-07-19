using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Repositry.Interfaces;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Craftsman")]
    public class CraftsmanWorksController : ControllerBase
    {
        private readonly ICraftsmanWorkRepo _craftsmanWorkRepo;
        public CraftsmanWorksController(ICraftsmanWorkRepo craftsmanWorkRepo)
        {
            _craftsmanWorkRepo = craftsmanWorkRepo;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllPreviousWorks()
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (craftsmanId == null) return Unauthorized("User ID not found.");

            var works = await _craftsmanWorkRepo.GetPreviousWork(craftsmanId);
            return Ok(works);
        }

        [HttpPost("upload-previous-work")]
        public async Task<IActionResult> UploadPreviousWork([FromForm] PreviousWorkDto previousWorkDto)
        {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(craftsmanId))
            {
                return Unauthorized(new { Message = "Craftsman not authenticated." });
            }

            await _craftsmanWorkRepo.AddPreviousWorkAsync(craftsmanId, previousWorkDto);

            return Ok(new { Message = "Previous work uploaded successfully." });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePreviousWork(int id, [FromForm] PreviousWorkDto dto)
        {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (craftsmanId == null) return Unauthorized("User ID not found.");

            var success = await _craftsmanWorkRepo.UpdatePreviousWorkAsync(id, dto, craftsmanId);
            return success ? Ok("Previous work updated successfully.") : NotFound("Previous work not found or unauthorized.");
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePreviousWork(int id)
        {
            var craftsmanId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (craftsmanId == null) return Unauthorized("User ID not found.");

            var success = await _craftsmanWorkRepo.DeletePreviousWorkAsync(id, craftsmanId);
            return success ? Ok("Previous work deleted successfully.") : NotFound("Previous work not found or unauthorized.");
        }
    }
}
