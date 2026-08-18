namespace PdfService.Application.Commands.Templates;

public sealed record GeneratePdfCommand(
    string TemplateCode,
    Dictionary<string, string> Data,
    Guid? CertificateId) : ICommand<Result<GeneratePdfResponse>>;

public sealed class GeneratePdfCommandValidator : AbstractValidator<GeneratePdfCommand>
{
    public GeneratePdfCommandValidator()
    {
        RuleFor(x => x.TemplateCode).NotEmpty();
        RuleFor(x => x.Data).NotNull();
    }
}

public sealed class GeneratePdfCommandHandler
    : ICommandHandler<GeneratePdfCommand, Result<GeneratePdfResponse>>
{
    private const string SignatureImageFileName = "signature.png";
    private const string DefaultSignReason = "Medical document approval";
    private const string DefaultSignLocation = "NeuroVisionAI";

    private readonly IPdfTemplateReadStore _readStore;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IPdfSigningService _pdfSigningService;
    private readonly ICertificateStorage _storage;
    private readonly ILogger<GeneratePdfCommandHandler> _logger;

    public GeneratePdfCommandHandler(
        IPdfTemplateReadStore readStore,
        IPdfGenerator pdfGenerator,
        IPdfSigningService pdfSigningService,
        ICertificateStorage storage,
        ILogger<GeneratePdfCommandHandler> logger)
    {
        _readStore = readStore;
        _pdfGenerator = pdfGenerator;
        _pdfSigningService = pdfSigningService;
        _storage = storage;
        _logger = logger;
    }

    public async Task<Result<GeneratePdfResponse>> Handle(
        GeneratePdfCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Generating PDF. Template={TemplateCode}",
            command.TemplateCode);

        var template = await _readStore.GetByCodeAsync(command.TemplateCode, cancellationToken);

        if (template is null)
        {
            return Result<GeneratePdfResponse>.Fail(
                "PDF template not found.",
                HttpStatusCode.NotFound);
        }

        await _readStore.LoadFieldsAsync(template, cancellationToken);

        if (template.RequiresSignature && command.CertificateId is null)
        {
            return Result<GeneratePdfResponse>.Fail(
                "Certificate is required for signing this document.",
                HttpStatusCode.BadRequest);
        }

        try
        {
            byte[]? signatureImage = null;
            if (template.RequiresSignature)
            {
                signatureImage = await _storage.TryReadSignatureImageAsync(
                    SignatureImageFileName,
                    cancellationToken);
            }

            var html = template.RenderHtml(command.Data, signatureImage);
            var generateResult = await _pdfGenerator.GenerateFromHtmlAsync(html, cancellationToken);

            if (!generateResult.IsSuccess)
            {
                return Result<GeneratePdfResponse>.Fail(
                    generateResult.Error,
                    generateResult.StatusCode);
            }

            var pdfBytes = generateResult.Value;
            var isSigned = false;

            if (template.RequiresSignature)
            {
                var position = _pdfSigningService.ResolvePosition(pdfBytes, template);
                var signedResult = await _pdfSigningService.SignPdfAsync(
                    pdfBytes,
                    command.CertificateId!.Value,
                    position,
                    DefaultSignReason,
                    DefaultSignLocation,
                    cancellationToken);

                if (!signedResult.IsSuccess)
                {
                    return Result<GeneratePdfResponse>.Fail(
                        signedResult.Error,
                        signedResult.StatusCode);
                }

                pdfBytes = signedResult.Value;
                isSigned = true;
            }

            return Result<GeneratePdfResponse>.Ok(
                new GeneratePdfResponse
                {
                    PdfBytes = pdfBytes,
                    IsSigned = isSigned,
                    CertificateId = command.CertificateId,
                    SignatureReason = isSigned ? DefaultSignReason : null,
                    SignatureLocation = isSigned ? DefaultSignLocation : null
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "PDF generation failed. Template={TemplateCode}",
                command.TemplateCode);

            return Result<GeneratePdfResponse>.Fail(
                "PDF generation failed.",
                HttpStatusCode.InternalServerError);
        }
    }
}
