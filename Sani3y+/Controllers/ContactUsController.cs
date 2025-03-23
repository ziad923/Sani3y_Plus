using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos;
using Sani3y_.Models;
using Sani3y_.Services;
using System.Security.Claims;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ContactUsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        [Authorize(Roles = "User")]
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
                // Check if the user exists (optional, depending on your requirements)
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == contactUsDto.Email);
                if (user == null)
                {
                    return Unauthorized("User Not Found");
                }

                // Generate a unique request number
                string requestNumber = $"REQ-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100, 999)}";

                // Map the DTO to the entity model
                var contactMessage = new ContactUs
                {
                    Name = contactUsDto.Name,
                    Email = contactUsDto.Email,
                    PhoneNumber = contactUsDto.PhoneNumber,
                    MessageContent = contactUsDto.MessageContent,
                    SentAt = DateTime.UtcNow,
                    RequestNumber = requestNumber
                };

                // Add the contact message to the database
                _context.ContactMessages.Add(contactMessage);
                await _context.SaveChangesAsync();

                // Return success response
                return Ok(new { RequestId = contactMessage.Id, RequestNumber = requestNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{requestNumber}")]
        public async Task<IActionResult> GetContactMessage(string requestNumber)
        {
            var message = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.RequestNumber == requestNumber);

            if (message == null)
            {
                return NotFound(new { Message = "Request not found" });
            }

            return Ok(message);
        }

    }
}
