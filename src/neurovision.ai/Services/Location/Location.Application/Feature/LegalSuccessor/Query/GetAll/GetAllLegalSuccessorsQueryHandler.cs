namespace LocationService.Application.Feature.LegalSuccessor.Query.GetAll;

public sealed class GetAllLegalSuccessorsQueryHandler
    : IQueryHandler<GetAllLegalSuccessorsQuery, Result<PaginatedResult<LegalSuccessorResponse>>>
{
    private readonly ILocationReadStore<LegalSuccessorResponse> reads;

    public GetAllLegalSuccessorsQueryHandler(ILocationReadStore<LegalSuccessorResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<LegalSuccessorResponse>>> Handle(
        GetAllLegalSuccessorsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(cancellationToken: cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<LegalSuccessorResponse>>.Ok(
            new PaginatedResult<LegalSuccessorResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
