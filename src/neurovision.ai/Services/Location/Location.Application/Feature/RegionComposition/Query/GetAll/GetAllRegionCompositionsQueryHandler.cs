namespace LocationService.Application.Feature.RegionComposition.Query.GetAll;

public sealed class GetAllRegionCompositionsQueryHandler
    : IQueryHandler<GetAllRegionCompositionsQuery, Result<PaginatedResult<RegionCompositionResponse>>>
{
    private readonly ILocationReadStore<RegionCompositionResponse> reads;

    public GetAllRegionCompositionsQueryHandler(ILocationReadStore<RegionCompositionResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<RegionCompositionResponse>>> Handle(
        GetAllRegionCompositionsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(cancellationToken: cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<RegionCompositionResponse>>.Ok(
            new PaginatedResult<RegionCompositionResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
