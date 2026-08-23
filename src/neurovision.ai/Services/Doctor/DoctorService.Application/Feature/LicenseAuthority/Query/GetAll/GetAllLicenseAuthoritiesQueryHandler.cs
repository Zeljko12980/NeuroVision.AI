namespace DoctorService.Application.Feature.LicenseAuthority.Query.GetAll;

public sealed class GetAllLicenseAuthoritiesQueryHandler
    : IQueryHandler<GetAllLicenseAuthoritiesQuery, Result<PaginatedResult<LicenseAuthorityResponse>>>
{
    private readonly IDoctorReadStore<LicenseAuthorityResponse> reads;
    private readonly ILogger<GetAllLicenseAuthoritiesQueryHandler> logger;

    public GetAllLicenseAuthoritiesQueryHandler(
        IDoctorReadStore<LicenseAuthorityResponse> reads,
        ILogger<GetAllLicenseAuthoritiesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<LicenseAuthorityResponse>>> Handle(
        GetAllLicenseAuthoritiesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get license authorities started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get license authorities succeeded. Count={Count}", total);

        return Result<PaginatedResult<LicenseAuthorityResponse>>.Ok(
            new PaginatedResult<LicenseAuthorityResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
