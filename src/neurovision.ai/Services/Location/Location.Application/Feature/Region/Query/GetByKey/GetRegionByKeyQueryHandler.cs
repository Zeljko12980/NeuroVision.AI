namespace LocationService.Application.Feature.Region.Query.GetByKey;

public sealed class GetRegionByKeyQueryHandler
    : IQueryHandler<GetRegionByKeyQuery, Result<RegionResponse>>
{
    private readonly ILocationReadStore<RegionResponse> reads;

    public GetRegionByKeyQueryHandler(ILocationReadStore<RegionResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<RegionResponse>> Handle(
        GetRegionByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.TypeCode, query.Code }, cancellationToken);

        if (item is null)
        {
            return Result<RegionResponse>.Fail(
                "Region not found.",
                HttpStatusCode.NotFound);
        }

        return Result<RegionResponse>.Ok(item);
    }
}
