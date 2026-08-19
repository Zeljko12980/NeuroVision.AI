namespace LocationService.Application.Feature.Country.Query.GetAll;

public sealed class GetAllCountriesQueryHandler
    : IQueryHandler<GetAllCountriesQuery, Result<PaginatedResult<CountryResponse>>>
{
    private readonly ILocationReadStore<CountryResponse> reads;

    public GetAllCountriesQueryHandler(ILocationReadStore<CountryResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<CountryResponse>>> Handle(
        GetAllCountriesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(new { request.Search, request.GovernmentTypeCode, request.IncludeCapital }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.GovernmentTypeCode, request.IncludeCapital, request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<CountryResponse>>.Ok(
            new PaginatedResult<CountryResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
