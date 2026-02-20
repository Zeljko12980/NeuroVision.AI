namespace MailService.Domain.Events
{
    public sealed class EmailFailedEvent : DomainEvent
    {
        public Guid EmailId { get; }
        public string ErrorMessage { get; }
        public int RetryCount { get; }
        public DateTime FailedAt { get; }

        public EmailFailedEvent(Guid emailId, string errorMessage, int retryCount, DateTime failedAt)
        {
            EmailId = emailId;
            ErrorMessage = errorMessage;
            RetryCount = retryCount;
            FailedAt = failedAt;
        }
    }
}
