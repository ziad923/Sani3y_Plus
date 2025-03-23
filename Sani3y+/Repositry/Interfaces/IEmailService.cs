using Sani3y_.Helpers;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
