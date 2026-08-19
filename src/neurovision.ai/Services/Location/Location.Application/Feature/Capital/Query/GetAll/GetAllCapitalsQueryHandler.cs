namespace LocationService.Application.Feature.Capital.Query.GetAll;

public sealed class GetAllCapitalsQueryHandler
    : IQueryHandler<GetAllCapitalsQuery, Result<PaginatedResult<CapitalResponse>>>
{
    private readonly ILocationReadStore<CapitalResponse> reads;

    public GetAllCapitalsQueryHandler(ILocationReadStore<CapitalResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<CapitalResponse>>> Handle(
        GetAllCapitalsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(cancellationToken: cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<CapitalResponse>>.Ok(
            new PaginatedResult<CapitalResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
