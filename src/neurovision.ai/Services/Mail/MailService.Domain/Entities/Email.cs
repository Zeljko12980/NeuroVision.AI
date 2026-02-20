namespace MailService.Domain.Entities
{
    public class Email
    {
        private readonly List<DomainEvent> _domainEvents = new();
        private readonly List<Attachment> _attachments = new();

        public Guid Id { get; private set; }
        public EmailAddress From { get; private set; }
        public List<EmailAddress> To { get; private set; }
        public List<EmailAddress> Cc { get; private set; }
        public List<EmailAddress> Bcc { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public bool IsHtml { get; private set; }
        public EmailPriority Priority { get; private set; }
        public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();

        public EmailStatus Status { get; private set; }
        public int RetryCount { get; private set; }
        public int MaxRetries { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? SentAt { get; private set; }
        public DateTime? FailedAt { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? TemplateId { get; private set; }

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private Email() { }

        private Email(
            EmailAddress from,
            List<EmailAddress> to,
            string subject,
            string body,
            bool isHtml,
            EmailPriority priority,
            int maxRetries = 3)
        {
            Id = Guid.NewGuid();
            From = from ?? throw new ArgumentNullException(nameof(from));
            To = to ?? throw new ArgumentNullException(nameof(to));
            Cc = new List<EmailAddress>();
            Bcc = new List<EmailAddress>();
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            IsHtml = isHtml;
            Priority = priority;
            Status = EmailStatus.Pending;
            RetryCount = 0;
            MaxRetries = maxRetries;
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new EmailCreatedEvent(Id, From, To, Subject));
        }

        public static Email Create(
            EmailAddress from,
            List<EmailAddress> to,
            string subject,
            string body,
            bool isHtml = true,
            EmailPriority priority = EmailPriority.Normal,
            int maxRetries = 3)
        {
            if (to == null || to.Count == 0)
                throw new ArgumentException("Email mora imati bar jednog primaoca.", nameof(to));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject ne može biti prazan.", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body ne može biti prazan.", nameof(body));

            return new Email(from, to, subject, body, isHtml, priority, maxRetries);
        }

        public void AddAttachment(Attachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));


            var totalSize = _attachments.Sum(a => a.SizeInBytes) + attachment.SizeInBytes;
            const long maxTotalSize = 25 * 1024 * 1024;

            if (totalSize > maxTotalSize)
                throw new InvalidOperationException($"Ukupna veličina attachmenata ne može biti veća od 25MB.");

            _attachments.Add(attachment);
        }

        public void AddCc(EmailAddress email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (!Cc.Contains(email))
                Cc.Add(email);
        }

        public void AddBcc(EmailAddress email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (!Bcc.Contains(email))
                Bcc.Add(email);
        }

        public void SetTemplate(string templateId)
        {
            if (string.IsNullOrWhiteSpace(templateId))
                throw new ArgumentException("Template ID ne može biti prazan.", nameof(templateId));

            TemplateId = templateId;
        }

        public void MarkAsSent()
        {
            if (Status == EmailStatus.Sent)
                throw new InvalidOperationException("Email je već poslat.");

            Status = EmailStatus.Sent;
            SentAt = DateTime.UtcNow;
            ErrorMessage = null;

            AddDomainEvent(new EmailSentEvent(Id, From, To, Subject, SentAt.Value));
        }

        public void MarkAsFailed(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message ne može biti prazan.", nameof(errorMessage));

            Status = EmailStatus.Failed;
            FailedAt = DateTime.UtcNow;
            ErrorMessage = errorMessage;
            RetryCount++;

            AddDomainEvent(new EmailFailedEvent(Id, errorMessage, RetryCount, FailedAt.Value));
        }

        public void MarkAsProcessing()
        {
            if (Status == EmailStatus.Sent)
                throw new InvalidOperationException("Email je već poslat.");

            Status = EmailStatus.Processing;
        }

        public bool CanRetry()
        {
            return Status == EmailStatus.Failed && RetryCount < MaxRetries;
        }

        public void Retry()
        {
            if (!CanRetry())
                throw new InvalidOperationException($"Email ne može biti retry-ovan. Status: {Status}, RetryCount: {RetryCount}/{MaxRetries}");

            Status = EmailStatus.Pending;
            ErrorMessage = null;

            AddDomainEvent(new EmailRetryingEvent(Id, RetryCount));
        }

        private void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
