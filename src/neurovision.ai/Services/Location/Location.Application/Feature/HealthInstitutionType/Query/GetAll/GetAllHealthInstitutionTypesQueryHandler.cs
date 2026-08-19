namespace LocationService.Application.Feature.HealthInstitutionType.Query.GetAll;

public sealed class GetAllHealthInstitutionTypesQueryHandler
    : IQueryHandler<GetAllHealthInstitutionTypesQuery, Result<PaginatedResult<HealthInstitutionTypeResponse>>>
{
    private readonly ILocationReadStore<HealthInstitutionTypeResponse> reads;

    public GetAllHealthInstitutionTypesQueryHandler(ILocationReadStore<HealthInstitutionTypeResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<HealthInstitutionTypeResponse>>> Handle(
        GetAllHealthInstitutionTypesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<HealthInstitutionTypeResponse>>.Ok(
            new PaginatedResult<HealthInstitutionTypeResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
