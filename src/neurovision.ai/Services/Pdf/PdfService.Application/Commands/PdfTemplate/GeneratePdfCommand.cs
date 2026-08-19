namespace PdfService.Application.Commands.Templates;

public sealed record GeneratePdfCommand(
    string TemplateCode,
    Dictionary<string, string> Data,
    Guid? CertificateId,
    Guid? UserId = null) : ICommand<Result<GeneratePdfResponse>>;

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
    private const string FallbackSignatureImageFileName = "signature.png";
    private const string DefaultSignReason = "Medical document approval";
    private const string DefaultSignLocation = "NeuroVisionAI";

    private readonly IPdfTemplateReadStore _readStore;
    private readonly ICertificateReadStore _certificateReadStore;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IPdfSigningService _pdfSigningService;
    private readonly ICertificateStorage _storage;
    private readonly ILogger<GeneratePdfCommandHandler> _logger;

    public GeneratePdfCommandHandler(
        IPdfTemplateReadStore readStore,
        ICertificateReadStore certificateReadStore,
        IPdfGenerator pdfGenerator,
        IPdfSigningService pdfSigningService,
        ICertificateStorage storage,
        ILogger<GeneratePdfCommandHandler> logger)
    {
        _readStore = readStore;
        _certificateReadStore = certificateReadStore;
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
            "Generating PDF. Template={TemplateCode}, UserId={UserId}",
            command.TemplateCode,
            command.UserId);

        var template = await _readStore.GetByCodeAsync(command.TemplateCode, cancellationToken);

        if (template is null)
        {
            return Result<GeneratePdfResponse>.Fail(
                "PDF template not found.",
                HttpStatusCode.NotFound);
        }

        await _readStore.LoadFieldsAsync(template, cancellationToken);

        Certificate? signingCertificate = null;
        if (template.RequiresSignature)
        {
            signingCertificate = await ResolveSigningCertificateAsync(command, cancellationToken);
            if (signingCertificate is null)
            {
                return Result<GeneratePdfResponse>.Fail(
                    "No signing certificate found for this user.",
                    HttpStatusCode.BadRequest);
            }
        }

        try
        {
            byte[]? signatureImage = null;
            if (template.RequiresSignature)
            {
                var signatureFile = signingCertificate!.SignatureImagePath
                    ?? FallbackSignatureImageFileName;
                signatureImage = await _storage.TryReadSignatureImageAsync(
                    signatureFile,
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
                    signingCertificate!.Id,
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
                    CertificateId = signingCertificate?.Id,
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

    private async Task<Certificate?> ResolveSigningCertificateAsync(
        GeneratePdfCommand command,
        CancellationToken cancellationToken)
    {
        if (command.UserId is { } userId && userId != Guid.Empty)
        {
            var byUser = await _certificateReadStore.GetByUserIdAsync(userId, cancellationToken);
            if (byUser is not null)
                return byUser;
        }

        if (command.CertificateId is { } certificateId && certificateId != Guid.Empty)
            return await _certificateReadStore.GetByIdAsync(certificateId, cancellationToken);

        return null;
    }
}
