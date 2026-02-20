namespace MailService.Domain.Events
{
    public sealed class EmailSentEvent : DomainEvent
    {
        public Guid EmailId { get; }
        public EmailAddress From { get; }
        public List<EmailAddress> To { get; }
        public string Subject { get; }
        public DateTime SentAt { get; }

        public EmailSentEvent(Guid emailId, EmailAddress from, List<EmailAddress> to, string subject, DateTime sentAt)
        {
            EmailId = emailId;
            From = from;
            To = to;
            Subject = subject;
            SentAt = sentAt;
        }
    }
}
