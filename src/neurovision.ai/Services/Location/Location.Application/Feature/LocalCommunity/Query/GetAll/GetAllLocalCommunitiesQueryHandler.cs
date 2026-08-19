namespace LocationService.Application.Feature.LocalCommunity.Query.GetAll;

public sealed class GetAllLocalCommunitiesQueryHandler
    : IQueryHandler<GetAllLocalCommunitiesQuery, Result<PaginatedResult<LocalCommunityResponse>>>
{
    private readonly ILocationReadStore<LocalCommunityResponse> reads;

    public GetAllLocalCommunitiesQueryHandler(ILocationReadStore<LocalCommunityResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<LocalCommunityResponse>>> Handle(
        GetAllLocalCommunitiesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<LocalCommunityResponse>>.Ok(
            new PaginatedResult<LocalCommunityResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
