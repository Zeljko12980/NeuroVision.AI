namespace LocationService.Application.Feature.HealthInstitution.Query.GetAll;

public sealed class GetAllHealthInstitutionsQueryHandler
    : IQueryHandler<GetAllHealthInstitutionsQuery, Result<PaginatedResult<HealthInstitutionResponse>>>
{
    private readonly ILocationReadStore<HealthInstitutionResponse> reads;

    public GetAllHealthInstitutionsQueryHandler(ILocationReadStore<HealthInstitutionResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<HealthInstitutionResponse>>> Handle(
        GetAllHealthInstitutionsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<HealthInstitutionResponse>>.Ok(
            new PaginatedResult<HealthInstitutionResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
