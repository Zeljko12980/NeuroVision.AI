namespace MailService.Domain.Events
{
    public sealed class EmailCreatedEvent : DomainEvent
    {
        public Guid EmailId { get; }
        public EmailAddress From { get; }
        public List<EmailAddress> To { get; }
        public string Subject { get; }

        public EmailCreatedEvent(Guid emailId, EmailAddress from, List<EmailAddress> to, string subject)
        {
            EmailId = emailId;
            From = from;
            To = to;
            Subject = subject;
        }
    }
}
