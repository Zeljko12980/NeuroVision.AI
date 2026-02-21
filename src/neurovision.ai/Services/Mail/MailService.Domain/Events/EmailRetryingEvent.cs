namespace MailService.Domain.Events
{
    public sealed class EmailRetryingEvent : DomainEvent
    {
        public Guid EmailId { get; }
        public int RetryAttempt { get; }

        public EmailRetryingEvent(Guid emailId, int retryAttempt)
        {
            EmailId = emailId;
            RetryAttempt = retryAttempt;
        }
    }
}
