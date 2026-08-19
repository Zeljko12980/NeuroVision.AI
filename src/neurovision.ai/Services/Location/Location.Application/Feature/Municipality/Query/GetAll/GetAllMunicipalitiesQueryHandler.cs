namespace LocationService.Application.Feature.Municipality.Query.GetAll;

public sealed class GetAllMunicipalitiesQueryHandler
    : IQueryHandler<GetAllMunicipalitiesQuery, Result<PaginatedResult<MunicipalityResponse>>>
{
    private readonly ILocationReadStore<MunicipalityResponse> reads;

    public GetAllMunicipalitiesQueryHandler(ILocationReadStore<MunicipalityResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<MunicipalityResponse>>> Handle(
        GetAllMunicipalitiesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<MunicipalityResponse>>.Ok(
            new PaginatedResult<MunicipalityResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
