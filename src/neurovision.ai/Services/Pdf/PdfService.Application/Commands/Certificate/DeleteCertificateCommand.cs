namespace PdfService.Application.Commands.Certificates;

public sealed record DeleteCertificateCommand(Guid Id) : ICommand<Result<bool>>;

public sealed class DeleteCertificateCommandHandler
    : ICommandHandler<DeleteCertificateCommand, Result<bool>>
{
    private readonly IRepository<Certificate, Guid> _repository;
    private readonly ICertificateStorage _storage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCertificateCommandHandler> _logger;

    public DeleteCertificateCommandHandler(
        IRepository<Certificate, Guid> repository,
        ICertificateStorage storage,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCertificateCommandHandler> logger)
    {
        _repository = repository;
        _storage = storage;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        DeleteCertificateCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting certificate. Id={CertificateId}", command.Id);

        var certificate = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (certificate is null)
        {
            _logger.LogWarning("Certificate not found. Id={CertificateId}", command.Id);
            return Result<bool>.Fail("Certificate not found.", HttpStatusCode.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(certificate.FilePath))
            await _storage.DeleteAsync(certificate.FilePath, cancellationToken);

        _repository.Delete(certificate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Certificate deleted successfully. Id={CertificateId}", command.Id);
        return Result<bool>.Ok(true);
    }
}
