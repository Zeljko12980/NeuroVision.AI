namespace LocationService.Application.Feature.GovernmentHistory.Query.GetAll;

public sealed class GetAllGovernmentHistoriesQueryHandler
    : IQueryHandler<GetAllGovernmentHistoriesQuery, Result<PaginatedResult<GovernmentHistoryResponse>>>
{
    private readonly ILocationReadStore<GovernmentHistoryResponse> reads;

    public GetAllGovernmentHistoriesQueryHandler(ILocationReadStore<GovernmentHistoryResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<GovernmentHistoryResponse>>> Handle(
        GetAllGovernmentHistoriesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(cancellationToken: cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<GovernmentHistoryResponse>>.Ok(
            new PaginatedResult<GovernmentHistoryResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
