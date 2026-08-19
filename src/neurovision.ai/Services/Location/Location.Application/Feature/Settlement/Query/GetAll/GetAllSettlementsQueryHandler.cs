namespace LocationService.Application.Feature.Settlement.Query.GetAll;

public sealed class GetAllSettlementsQueryHandler
    : IQueryHandler<GetAllSettlementsQuery, Result<PaginatedResult<SettlementResponse>>>
{
    private readonly ILocationReadStore<SettlementResponse> reads;

    public GetAllSettlementsQueryHandler(ILocationReadStore<SettlementResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<SettlementResponse>>> Handle(
        GetAllSettlementsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<SettlementResponse>>.Ok(
            new PaginatedResult<SettlementResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
