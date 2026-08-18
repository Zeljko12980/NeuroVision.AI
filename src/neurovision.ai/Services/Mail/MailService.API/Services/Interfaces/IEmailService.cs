namespace MailService.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string toEmail, string subject, string body);
        Task SendEmailWithAttachment(string toEmail, string subject, string body, byte[] attachmentBytes, string attachmentFileName);
    }
}
