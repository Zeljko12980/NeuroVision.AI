namespace PdfService.Application.Commands.Certificates;

public sealed record CreateCertificateCommand(
    string Name,
    string? Password,
    byte[] FileContent,
    string FileName) : ICommand<Result<CertificateResponse>>;

public sealed class CreateCertificateCommandValidator : AbstractValidator<CreateCertificateCommand>
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    private static readonly string[] AllowedExtensions =
    [
        ".pfx", ".p12", ".cer", ".crt", ".der"
    ];

    public CreateCertificateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.FileContent)
            .NotEmpty()
            .WithMessage("The file is empty or was not provided.")
            .Must(content => content.Length <= MaxFileSizeBytes)
            .WithMessage($"The file exceeds the maximum allowed size of {MaxFileSizeBytes / 1024 / 1024} MB.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(HasAllowedExtension)
            .WithMessage($"File extension is not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}.");
    }

    private static bool HasAllowedExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrWhiteSpace(extension)
            && AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
}

public sealed class CreateCertificateCommandHandler
    : ICommandHandler<CreateCertificateCommand, Result<CertificateResponse>>
{
    private readonly ICertificateFileParser _parser;
    private readonly ICertificateStorage _storage;
    private readonly ICertificatePasswordProtector _passwordProtector;
    private readonly IRepository<Certificate, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCertificateCommandHandler> _logger;

    public CreateCertificateCommandHandler(
        ICertificateFileParser parser,
        ICertificateStorage storage,
        ICertificatePasswordProtector passwordProtector,
        IRepository<Certificate, Guid> repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCertificateCommandHandler> logger)
    {
        _parser = parser;
        _storage = storage;
        _passwordProtector = passwordProtector;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CertificateResponse>> Handle(
        CreateCertificateCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading certificate. Name={Name}", command.Name);

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

        string relativePath;
        try
        {
            relativePath = await _storage.SaveAsync(
                command.FileContent,
                command.FileName,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save certificate file to storage.");
            return Result<CertificateResponse>.Fail(
                "Failed to save the certificate file.",
                HttpStatusCode.InternalServerError);
        }

        Certificate certificate;
        try
        {
            certificate = Certificate.Create(
                command.Name,
                parsed.Subject,
                parsed.Issuer,
                parsed.Thumbprint,
                parsed.SerialNumber,
                parsed.ValidFrom,
                parsed.ValidTo,
                command.FileName,
                relativePath,
                _passwordProtector.Protect(command.Password ?? string.Empty));
        }
        catch (ArgumentException ex)
        {
            await _storage.DeleteAsync(relativePath, cancellationToken);
            return Result<CertificateResponse>.Fail(ex.Message, HttpStatusCode.BadRequest);
        }

        try
        {
            await _repository.AddAsync(certificate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save certificate record. Cleaning up stored file.");
            await _storage.DeleteAsync(relativePath, cancellationToken);
            return Result<CertificateResponse>.Fail(
                "Failed to save the certificate record.",
                HttpStatusCode.InternalServerError);
        }

        _logger.LogInformation("Certificate uploaded successfully. Id={CertificateId}", certificate.Id);
        return Result<CertificateResponse>.Ok(certificate.ToResponse());
    }
}
