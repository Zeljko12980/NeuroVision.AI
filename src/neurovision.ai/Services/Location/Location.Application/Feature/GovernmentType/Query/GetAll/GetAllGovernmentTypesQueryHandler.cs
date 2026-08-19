namespace LocationService.Application.Feature.GovernmentType.Query.GetAll;

public sealed class GetAllGovernmentTypesQueryHandler
    : IQueryHandler<GetAllGovernmentTypesQuery, Result<PaginatedResult<GovernmentTypeResponse>>>
{
    private readonly ILocationReadStore<GovernmentTypeResponse> reads;

    public GetAllGovernmentTypesQueryHandler(ILocationReadStore<GovernmentTypeResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<GovernmentTypeResponse>>> Handle(
        GetAllGovernmentTypesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<GovernmentTypeResponse>>.Ok(
            new PaginatedResult<GovernmentTypeResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
