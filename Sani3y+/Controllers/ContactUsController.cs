using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos;
using Sani3y_.Helpers;
using Sani3y_.Models;
using Sani3y_.Services;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User, Craftsman")]
    public class ContactUsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ContactUsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitContact([FromBody] ContactUsDto contactUsDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (contactUsDto == null)
            {
                return BadRequest("Invalid message data.");
            }

            try
            {
                string requestNumber = RequestNumberGenerator.GenerateRequestNumber();

				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var contactMessage = new ContactUs
                {
                    Name = contactUsDto.Name,
                    Email = contactUsDto.Email,
                    PhoneNumber = contactUsDto.PhoneNumber,
                    MessageContent = contactUsDto.MessageContent,
                    SentAt = DateTime.UtcNow,
                    RequestNumber = requestNumber,
                    UserId = userId
                };

                _context.ContactMessages.Add(contactMessage);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "تم إرسال الرسالة بنجاح. سنتواصل معك خلال ٢٤ ساعة.",
                    RequestNumber = requestNumber
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "حدث خطأ أثناء إرسال الرسالة." });
            }
        }


    }
}
