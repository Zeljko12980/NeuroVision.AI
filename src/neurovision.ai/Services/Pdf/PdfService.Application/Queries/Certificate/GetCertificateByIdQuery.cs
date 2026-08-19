namespace PdfService.Application.Queries.Certificates;

public sealed record GetCertificateByIdQuery(Guid Id) : IQuery<Result<CertificateResponse>>;

public sealed class GetCertificateByIdQueryHandler
    : IQueryHandler<GetCertificateByIdQuery, Result<CertificateResponse>>
{
    private readonly IRepository<Certificate, Guid> _repository;
    private readonly ILogger<GetCertificateByIdQueryHandler> _logger;

    public GetCertificateByIdQueryHandler(
        IRepository<Certificate, Guid> repository,
        ILogger<GetCertificateByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<CertificateResponse>> Handle(
        GetCertificateByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting certificate. Id={CertificateId}", query.Id);

        var certificate = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (certificate is null)
        {
            _logger.LogWarning("Certificate not found. Id={CertificateId}", query.Id);
            return Result<CertificateResponse>.Fail("Certificate not found.", HttpStatusCode.NotFound);
        }

        return Result<CertificateResponse>.Ok(certificate.ToResponse());
    }
}
