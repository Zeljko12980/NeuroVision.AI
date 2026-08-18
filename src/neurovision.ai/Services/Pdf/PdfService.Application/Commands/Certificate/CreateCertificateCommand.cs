namespace PdfService.Application.Commands.Certificates;

public sealed record CreateCertificateCommand(
    Guid UserId,
    string Name,
    string? Password,
    byte[] FileContent,
    string FileName,
    byte[] SignatureImageContent,
    string SignatureImageFileName) : ICommand<Result<CertificateResponse>>;

public sealed class CreateCertificateCommandValidator : AbstractValidator<CreateCertificateCommand>
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;
    private const long MaxSignatureImageBytes = 2 * 1024 * 1024;

    private static readonly string[] AllowedExtensions =
    [
        ".pfx", ".p12", ".cer", ".crt", ".der"
    ];

    private static readonly string[] AllowedSignatureExtensions =
    [
        ".png", ".jpg", ".jpeg", ".webp"
    ];

    public CreateCertificateCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.FileContent)
            .NotEmpty()
            .WithMessage("The file is empty or was not provided.")
            .Must(content => content.Length <= MaxFileSizeBytes)
            .WithMessage($"The file exceeds the maximum allowed size of {MaxFileSizeBytes / 1024 / 1024} MB.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(fileName => HasAllowedExtension(fileName, AllowedExtensions))
            .WithMessage($"File extension is not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}.");

        RuleFor(x => x.SignatureImageContent)
            .NotEmpty()
            .WithMessage("The signature image is required.")
            .Must(content => content.Length <= MaxSignatureImageBytes)
            .WithMessage("The signature image exceeds the maximum allowed size of 2 MB.");

        RuleFor(x => x.SignatureImageFileName)
            .NotEmpty()
            .Must(fileName => HasAllowedExtension(fileName, AllowedSignatureExtensions))
            .WithMessage($"Signature image type is not allowed. Allowed types: {string.Join(", ", AllowedSignatureExtensions)}.");
    }

    private static bool HasAllowedExtension(string fileName, IEnumerable<string> allowed)
    {
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrWhiteSpace(extension)
            && allowed.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
}

public sealed class CreateCertificateCommandHandler
    : ICommandHandler<CreateCertificateCommand, Result<CertificateResponse>>
{
    private readonly ICertificateFileParser _parser;
    private readonly ICertificateStorage _storage;
    private readonly ICertificatePasswordProtector _passwordProtector;
    private readonly ICertificateReadStore _readStore;
    private readonly IRepository<Certificate, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCertificateCommandHandler> _logger;

    public CreateCertificateCommandHandler(
        ICertificateFileParser parser,
        ICertificateStorage storage,
        ICertificatePasswordProtector passwordProtector,
        ICertificateReadStore readStore,
        IRepository<Certificate, Guid> repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCertificateCommandHandler> logger)
    {
        _parser = parser;
        _storage = storage;
        _passwordProtector = passwordProtector;
        _readStore = readStore;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CertificateResponse>> Handle(
        CreateCertificateCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Uploading certificate. Name={Name}, UserId={UserId}",
            command.Name,
            command.UserId);

        if (await _readStore.ExistsForUserAsync(command.UserId, cancellationToken))
        {
            return Result<CertificateResponse>.Fail(
                "A signing certificate already exists for this user.",
                HttpStatusCode.Conflict);
        }

        var parseResult = _parser.Parse(command.FileContent, command.Password);
        if (!parseResult.IsSuccess)
            return Result<CertificateResponse>.Fail(parseResult.Error, parseResult.StatusCode);

        var parsed = parseResult.Value;
        if (parsed.ValidTo < DateTime.UtcNow)
        {
            return Result<CertificateResponse>.Fail(
                "The certificate has already expired.",
                HttpStatusCode.BadRequest);
        }

        string? relativePath = null;
        string? signaturePath = null;
        try
        {
            relativePath = await _storage.SaveAsync(
                command.FileContent,
                command.FileName,
                cancellationToken);

            signaturePath = await _storage.SaveSignatureImageAsync(
                command.SignatureImageContent,
                command.SignatureImageFileName,
                cancellationToken);

            var certificate = Certificate.Create(
                command.Name,
                parsed.Subject,
                parsed.Issuer,
                parsed.Thumbprint,
                parsed.SerialNumber,
                parsed.ValidFrom,
                parsed.ValidTo,
                command.FileName,
                relativePath,
                _passwordProtector.Protect(command.Password ?? string.Empty),
                userId: command.UserId,
                signatureImagePath: signaturePath);

            await _repository.AddAsync(certificate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Certificate uploaded successfully. Id={CertificateId}", certificate.Id);
            return Result<CertificateResponse>.Ok(certificate.ToResponse());
        }
        catch (ArgumentException ex)
        {
            await CleanupAsync(relativePath, signaturePath, cancellationToken);
            return Result<CertificateResponse>.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
        catch (Exception ex) when (relativePath is null)
        {
            _logger.LogError(ex, "Failed to save certificate file to storage.");
            return Result<CertificateResponse>.Fail(
                "Failed to save the certificate file.",
                HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save certificate. Cleaning up stored files.");
            await CleanupAsync(relativePath, signaturePath, cancellationToken);
            return Result<CertificateResponse>.Fail(
                "Failed to save the certificate record.",
                HttpStatusCode.InternalServerError);
        }
    }

    private async Task CleanupAsync(
        string? certificatePath,
        string? signaturePath,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(certificatePath))
            await _storage.DeleteAsync(certificatePath, cancellationToken);

        if (!string.IsNullOrWhiteSpace(signaturePath))
            await _storage.DeleteAsync(signaturePath, cancellationToken);
    }
}
