namespace MailService.Domain.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(Email email, CancellationToken cancellationToken = default);
    }
}
