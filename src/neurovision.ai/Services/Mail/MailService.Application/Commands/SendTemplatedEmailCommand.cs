namespace MailService.Application.Commands;

public record SendTemplatedEmailCommand(
    string To,
    string TemplateId,
    IReadOnlyDictionary<string, string> Placeholders) : ICommand<Result>;

public class SendTemplatedEmailCommandValidator : AbstractValidator<SendTemplatedEmailCommand>
{
    public SendTemplatedEmailCommandValidator()
    {
        RuleFor(x => x.To)
            .NotEmpty().WithMessage("Recipient email is required.")
            .Must(EmailAddress.IsValid).WithMessage("Recipient email is invalid.");

        RuleFor(x => x.TemplateId)
            .NotEmpty().WithMessage("Template is required.")
            .Must(id => EmailTemplates.TryGet(id, out _))
            .WithMessage("Unknown email template.");

        RuleFor(x => x.Placeholders)
            .NotNull().WithMessage("Placeholders are required.");
    }
}

public class SendTemplatedEmailCommandHandler
    : ICommandHandler<SendTemplatedEmailCommand, Result>
{
    private readonly IDocumentGenerator _documentGenerator;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<SendTemplatedEmailCommandHandler> _logger;

    public SendTemplatedEmailCommandHandler(
        IDocumentGenerator documentGenerator,
        IEmailSender emailSender,
        ILogger<SendTemplatedEmailCommandHandler> logger)
    {
        _documentGenerator = documentGenerator;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<Result> Handle(SendTemplatedEmailCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var to = EmailAddress.Create(command.To);
            var template = EmailTemplates.Get(command.TemplateId);
            template.EnsurePlaceholders(command.Placeholders);

            _logger.LogInformation(
                "Sending templated email. TemplateId={TemplateId}, To={To}",
                template.Id,
                to.Value);

            var documentResult = await _documentGenerator.GenerateAsync(
                template.DocumentTemplateCode,
                command.Placeholders,
                cancellationToken);

            if (!documentResult.IsSuccess)
            {
                _logger.LogWarning(
                    "Document generation failed. TemplateId={TemplateId}, Error={Error}",
                    template.Id,
                    documentResult.Error);
                return Result.Fail(documentResult.Error, documentResult.StatusCode);
            }

            var message = EmailMessage.Create(
                to,
                template.Subject,
                template.HtmlBody,
                [EmailAttachment.Pdf(template.AttachmentFileName, documentResult.Value)]);

            var sendResult = await _emailSender.SendAsync(message, cancellationToken);
            if (!sendResult.IsSuccess)
            {
                _logger.LogWarning(
                    "Email send failed. TemplateId={TemplateId}, To={To}, Error={Error}",
                    template.Id,
                    to.Value,
                    sendResult.Error);
                return sendResult;
            }

            _logger.LogInformation(
                "Templated email sent. TemplateId={TemplateId}, To={To}",
                template.Id,
                to.Value);

            return Result.Ok();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Templated email rejected. TemplateId={TemplateId}", command.TemplateId);
            return Result.Fail(ex.Message);
        }
    }
}
