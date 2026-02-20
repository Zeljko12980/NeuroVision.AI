namespace MailService.Application.Commands
{
    public class SendEmailCommand : ICommand<SendEmailResponse>
    {
        public string From { get; set; } = string.Empty;
        public List<string> To { get; set; } = new();
        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
        public string? Priority { get; set; }
        public List<AttachmentResponse>? Attachments { get; set; }
        public string? TemplateId { get; set; }
        public Dictionary<string, object>? TemplateModel { get; set; }
    }

    public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
    {
        public SendEmailCommandValidator()
        {
            RuleFor(x => x.From)
                .NotEmpty().WithMessage("Sender address is required.")
                .EmailAddress().WithMessage("Sender address must be a valid email.");

            RuleFor(x => x.To)
                .NotEmpty().WithMessage("At least one recipient is required.")
                .Must(list => list.All(addr =>
                    !string.IsNullOrWhiteSpace(addr)))
                .WithMessage("Recipient addresses must be valid emails.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Body cannot be empty.");
        }
    }

    public class SendEmailHandler : IRequestHandler<SendEmailCommand, SendEmailResponse>
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailSender _emailSender;
        private readonly ITemplateEngine _templateEngine;
        private readonly IEventStore _eventStore;

        public SendEmailHandler(
            IEmailRepository emailRepository,
            IEmailSender emailSender,
            ITemplateEngine templateEngine,
            IEventStore eventStore)
        {
            _emailRepository = emailRepository;
            _emailSender = emailSender;
            _templateEngine = templateEngine;
            _eventStore = eventStore;
        }

        public async Task<SendEmailResponse> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var from = EmailAddress.Create(request.From);
                var to = request.To.Select(EmailAddress.Create).ToList();

                string body = request.Body;
                if (!string.IsNullOrEmpty(request.TemplateId))
                {
                    var templateExists = await _templateEngine.TemplateExistsAsync(request.TemplateId, cancellationToken);
                    if (!templateExists)
                    {
                        return new SendEmailResponse
                        {
                            Success = false,
                            Message = $"Template '{request.TemplateId}' does not exist."
                        };
                    }

                    body = await _templateEngine.RenderAsync(
                        request.TemplateId,
                        request.TemplateModel ?? new Dictionary<string, object>(),
                        cancellationToken);
                }

                var priority = EmailPriority.Normal;
                if (!string.IsNullOrEmpty(request.Priority) &&
                    Enum.TryParse<EmailPriority>(request.Priority, true, out var parsedPriority))
                {
                    priority = parsedPriority;
                }

                var email = Email.Create(from, to, request.Subject, body, request.IsHtml, priority);

                if (request.Cc != null)
                {
                    foreach (var cc in request.Cc)
                    {
                        email.AddCc(EmailAddress.Create(cc));
                    }
                }

                if (request.Bcc != null)
                {
                    foreach (var bcc in request.Bcc)
                    {
                        email.AddBcc(EmailAddress.Create(bcc));
                    }
                }

                if (request.Attachments != null)
                {
                    foreach (var att in request.Attachments)
                    {
                        var attachment = Attachment.Create(att.FileName, att.Content, att.ContentType);
                        email.AddAttachment(attachment);
                    }
                }

                if (!string.IsNullOrEmpty(request.TemplateId))
                {
                    email.SetTemplate(request.TemplateId);
                }

                await _emailRepository.AddAsync(email, cancellationToken);

                foreach (var domainEvent in email.DomainEvents)
                {
                    await _eventStore.SaveEventAsync(domainEvent, cancellationToken);
                }

                email.ClearDomainEvents();

                try
                {
                    email.MarkAsProcessing();
                    await _emailRepository.UpdateAsync(email, cancellationToken);

                    await _emailSender.SendAsync(email, cancellationToken);

                    email.MarkAsSent();
                    await _emailRepository.UpdateAsync(email, cancellationToken);

                    foreach (var domainEvent in email.DomainEvents)
                    {
                        await _eventStore.SaveEventAsync(domainEvent, cancellationToken);
                    }

                    return new SendEmailResponse
                    {
                        EmailId = email.Id,
                        Success = true,
                        Message = "Email successfully sent."
                    };
                }
                catch (Exception ex)
                {
                    email.MarkAsFailed(ex.Message);
                    await _emailRepository.UpdateAsync(email, cancellationToken);

                    foreach (var domainEvent in email.DomainEvents)
                    {
                        await _eventStore.SaveEventAsync(domainEvent, cancellationToken);
                    }

                    return new SendEmailResponse
                    {
                        EmailId = email.Id,
                        Success = false,
                        Message = $"Email sending failed: {ex.Message}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new SendEmailResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}