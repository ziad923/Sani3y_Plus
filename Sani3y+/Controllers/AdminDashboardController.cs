using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos;
using Sani3y_.Dtos.Account;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Enums;
using Sani3y_.Models;
using Sani3y_.Repositry;
using Sani3y_.Repositry.Interfaces;
using Sani3y_.Services;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IContactMessageRepo _contactMessageRepo;
        public AdminDashboardController(
            INotificationService notificationService,
           IContactMessageRepo contactMessageRep)
        {
            _notificationService = notificationService;
            _contactMessageRepo = contactMessageRep;
        }

        [HttpGet("GetContactMessages")]
        public async Task<ActionResult<List<ContactUsResponseDto>>> GetContactMessages(bool onlyUnresolved = true)
        {
           var messages = await _contactMessageRepo.GetContactMessagesAsync(onlyUnresolved);
            return Ok(messages);
        }

        //[HttpGet("GetContactMessages")]
        //public async Task<ActionResult<List<ContactUs>>> GetContactMessages()
        //{
        //    var messages = await _context.ContactMessages
        //         .Include(c => c.User)
        //         .OrderByDescending(c => c.SentAt)
        //         .Select(c => new ContactUsResponseDto
        //         {
        //             RequestNumber = c.RequestNumber,
        //             Name = c.Name,
        //             Email = c.Email,
        //             PhoneNumber = c.PhoneNumber,
        //             MessageContent = c.MessageContent,
        //             SentAt = c.SentAt,
        //             IsResolved = c.IsResolved,
        //             ResolvedAt = c.ResolvedAt,
        //             UserId = c.UserId
        //         })
        //        .ToListAsync();

        //    return Ok(messages);
        //}

        [HttpPost("resolve/{requestNumber}")]
        public async Task<IActionResult> ResolveContactMessage(string requestNumber)
        {
            var contactMessage = await _contactMessageRepo.GetContactMessageByRequestNumberAsync(requestNumber);

            if (contactMessage == null)
                return NotFound("Message not found.");

            contactMessage.IsResolved = true;
            contactMessage.ResolvedAt = DateTime.UtcNow;
            await _contactMessageRepo.UpdateAsync(contactMessage);

            
            await _notificationService.SendNotificationAsync(
                userId: contactMessage.UserId,
                title: "متابعة الشكوي",
                message: $"عميلنا العزيز،\nتم حل مشكلتك بنجاحاً (رقم الطلب: {contactMessage.RequestNumber}).\nإذا واجهتك أية مشاكل أخرى لا تردد في التواصل معنا."
            );

            return Ok(new { Message = "Message marked as resolved and user notified." });
        }
    }
}
