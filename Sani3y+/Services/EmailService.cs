using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Sani3y_.Repositry.Interfaces;
using Microsoft.Extensions.Configuration;
namespace Sani3y_.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(
             emailSettings["FromName"],   // Display name (e.g., "Sanal3y plus")
            emailSettings["FromEmail"]   // Email address (e.g., "help@sanal3yplus.com")
  ));
            emailMessage.To.Add(new MailboxAddress(email, email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), true);
                    await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Log the exception to get more information on the error
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;  // Rethrow or handle the exception accordingly
            }
        }
    }
}
