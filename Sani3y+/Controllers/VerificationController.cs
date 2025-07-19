using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Dtos.Verification;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verificationService;
        private readonly UserManager<AppUser> _userManager;

        public VerificationController(IVerificationService verificationService, UserManager<AppUser> userManager)
        {
            _verificationService = verificationService;
            _userManager = userManager;
        }
        [HttpPost("submit")]
        [Authorize(Roles = "Craftsman")]
        public async Task<IActionResult> Submit([FromForm] SubmitVerificationRequestDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _verificationService.SubmitVerificationRequestAsync(userId, dto);
                return Ok(new { message = "تم إرسال طلب التوثيق بنجاح." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingRequests()
        {
            return Ok(await _verificationService.GetPendingRequestsAsync());
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve([FromRoute] int id)
        {
            var result = await _verificationService.ApproveRequestAsync(id);
            return result ? Ok(new { message = "تمت الموافقة على التوثيق." }) : NotFound();
        }

        [HttpPost("reject/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject([FromRoute] int id)
        {
            var result = await _verificationService.RejectRequestAsync(id);
            return result ? Ok(new { message = "تم رفض طلب التوثيق." }) : NotFound();
        }
    }
}
