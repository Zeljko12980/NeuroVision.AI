namespace MailService.Domain.Interfaces
{
    public interface IEmailRepository
    {
        Task<Email?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Email>> GetPendingEmailsAsync(int batchSize = 10, CancellationToken cancellationToken = default);
        Task<List<Email>> GetFailedEmailsForRetryAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Email email, CancellationToken cancellationToken = default);
        Task UpdateAsync(Email email, CancellationToken cancellationToken = default);
        Task<int> GetEmailCountByStatusAsync(EmailStatus status, CancellationToken cancellationToken = default);
    }
}
