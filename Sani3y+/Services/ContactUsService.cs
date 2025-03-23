using Sani3y_.Data;
using Sani3y_.Dtos;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{
    public class ContactUsService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public ContactUsService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<bool> SubmitContactUs(ContactUsDto contactDto)
        {
            var contactMessage = new ContactUs
            {
                Name = contactDto.Name,
                Email = contactDto.Email,
                PhoneNumber = contactDto.PhoneNumber,
                MessageContent = contactDto.MessageContent
            };

            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync();

            // Send Email
            var subject = "New Contact Us Message";
            var body = $"<p><strong>Name:</strong> {contactDto.Name}</p>" +
                       $"<p><strong>Email:</strong> {contactDto.Email}</p>" +
                       $"<p><strong>Phone:</strong> {contactDto.PhoneNumber}</p>" +
                       $"<p><strong>Message:</strong> {contactDto.MessageContent}</p>";

            await _emailService.SendEmailAsync("admin@yourdomain.com", subject, body);
            return true;
        }

    }
}
