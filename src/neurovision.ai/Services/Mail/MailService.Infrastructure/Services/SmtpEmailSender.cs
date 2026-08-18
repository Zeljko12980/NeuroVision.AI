using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MailService.Infrastructure.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpSettings> settings, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<Result> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            mime.To.Add(MailboxAddress.Parse(message.To.Value));
            mime.Subject = message.Subject;

            var body = new BodyBuilder { HtmlBody = message.HtmlBody };
            foreach (var attachment in message.Attachments)
            {
                body.Attachments.Add(
                    attachment.FileName,
                    attachment.Content,
                    MimeKit.ContentType.Parse(attachment.ContentType));
            }

            mime.Body = body.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _settings.Server,
                _settings.Port,
                ResolveSocketOptions(),
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(_settings.Username))
            {
                await client.AuthenticateAsync(
                    _settings.Username,
                    _settings.Password,
                    cancellationToken);
            }

            await client.SendAsync(mime, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("SMTP message sent. To={To}, Subject={Subject}", message.To.Value, message.Subject);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP send failed. To={To}", message.To.Value);
            return Result.Fail("Failed to send email.");
        }
    }

    private SecureSocketOptions ResolveSocketOptions()
    {
        if (!_settings.EnableSsl)
            return SecureSocketOptions.None;

        return _settings.Port == 465
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;
    }
}
