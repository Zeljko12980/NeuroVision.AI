namespace LocationService.Application.Feature.CountryComposition.Query.GetAll;

public sealed class GetAllCountryCompositionsQueryHandler
    : IQueryHandler<GetAllCountryCompositionsQuery, Result<PaginatedResult<CountryCompositionResponse>>>
{
    private readonly ILocationReadStore<CountryCompositionResponse> reads;

    public GetAllCountryCompositionsQueryHandler(ILocationReadStore<CountryCompositionResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<CountryCompositionResponse>>> Handle(
        GetAllCountryCompositionsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(cancellationToken: cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<CountryCompositionResponse>>.Ok(
            new PaginatedResult<CountryCompositionResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
