using MailKit.Net.Smtp;
using MailKit.Security;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;
using Polly.Retry;
namespace MailService.Infrastructure.Smtp
{
    public class GoogleSmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<GoogleSmtpEmailSender> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public GoogleSmtpEmailSender(
            IOptions<SmtpSettings> settings,
            ILogger<GoogleSmtpEmailSender> logger)
        {
            _settings = settings.Value;
            _logger = logger;


            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            "Retry {RetryCount} za slanje emaila nakon {Delay}s. Greška: {Error}",
                            retryCount, timeSpan.TotalSeconds, exception.Message);
                    });
        }

        public async Task SendAsync(Email email, CancellationToken cancellationToken = default)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using var message = CreateMimeMessage(email);
                // ... inside SendAsync method, replace the following lines:

                // using var client = new System.Net.Mail.SmtpClient();
                using var client = new SmtpClient();

                try
                {
                    _logger.LogInformation("Konektujem se na SMTP server: {Host}:{Port}", _settings.Host, _settings.Port);

                    // await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);
                    await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);
                    // await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
                    await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);

                    _logger.LogInformation("Šaljem email: {EmailId} - {Subject}", email.Id, email.Subject);
                    // await client.SendAsync(message, cancellationToken);
                    await client.SendAsync(message, cancellationToken);

                    _logger.LogInformation("Email uspešno poslat: {EmailId}", email.Id);

                    // await client.DisconnectAsync(true, cancellationToken);
                    await client.DisconnectAsync(true, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška pri slanju emaila: {EmailId}", email.Id);
                    throw;
                }
            });
        }

        private MimeMessage CreateMimeMessage(Email email)
        {
            var message = new MimeMessage();

            // From
            message.From.Add(MailboxAddress.Parse(email.From.Value));

            // To
            foreach (var to in email.To)
            {
                message.To.Add(MailboxAddress.Parse(to.Value));
            }

            // Cc
            foreach (var cc in email.Cc)
            {
                message.Cc.Add(MailboxAddress.Parse(cc.Value));
            }

            // Bcc
            foreach (var bcc in email.Bcc)
            {
                message.Bcc.Add(MailboxAddress.Parse(bcc.Value));
            }

            // Subject
            message.Subject = email.Subject;

            // Priority
            message.Priority = email.Priority switch
            {
                Domain.ValueObjects.EmailPriority.Low => MessagePriority.NonUrgent,
                Domain.ValueObjects.EmailPriority.High => MessagePriority.Urgent,
                Domain.ValueObjects.EmailPriority.Urgent => MessagePriority.Urgent,
                _ => MessagePriority.Normal
            };

            // Body builder
            var builder = new BodyBuilder();

            if (email.IsHtml)
            {
                builder.HtmlBody = email.Body;
            }
            else
            {
                builder.TextBody = email.Body;
            }

            // Attachments
            foreach (var attachment in email.Attachments)
            {
                builder.Attachments.Add(
                    attachment.FileName,
                    attachment.Content,
                    MimeKit.ContentType.Parse(attachment.ContentType));
            }

            message.Body = builder.ToMessageBody();

            return message;
        }
    }
}
