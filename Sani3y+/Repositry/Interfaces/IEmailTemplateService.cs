namespace Sani3y_.Repositry.Interfaces
{
    public interface IEmailTemplateService
    {
        (string Subject, string Body) GetResetPasswordEmail(string resetUrl);
    }
}
