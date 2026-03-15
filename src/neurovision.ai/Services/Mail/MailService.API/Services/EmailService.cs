namespace MailService.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _config;

        public EmailService(IOptions<SmtpSettings> config)
        {
            _config = config.Value;
        }

        public async Task SendEmail(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient
            {
                Host = _config.Server,
                Port = _config.Port,
                Credentials = new NetworkCredential(_config.Username, _config.Password),
                EnableSsl = _config.EnableSsl
            };

            var mail = new MailMessage(_config.SenderEmail, toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }

        public async Task SendEmailWithAttachment(string toEmail, string subject, string body, byte[] attachmentBytes, string attachmentFileName)
        {
            using var client = new SmtpClient
            {
                Host = _config.Server,
                Port = _config.Port,
                Credentials = new NetworkCredential(_config.Username, _config.Password),
                EnableSsl = _config.EnableSsl
            };

            using var mail = new MailMessage(_config.SenderEmail, toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            using var stream = new MemoryStream(attachmentBytes);
            var attachment = new Attachment(stream, attachmentFileName, "application/pdf");

            mail.Attachments.Add(attachment);

            await client.SendMailAsync(mail);
        }

    }
}
