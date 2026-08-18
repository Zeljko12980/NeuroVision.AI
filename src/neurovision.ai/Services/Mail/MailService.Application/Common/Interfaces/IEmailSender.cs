namespace MailService.Application.Common.Interfaces;

public interface IEmailSender
{
    Task<Result> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
