namespace LocationService.Application.Feature.RegionType.Query.GetByKey;

public sealed class GetRegionTypeByKeyQueryHandler
    : IQueryHandler<GetRegionTypeByKeyQuery, Result<RegionTypeResponse>>
{
    private readonly ILocationReadStore<RegionTypeResponse> reads;

    public GetRegionTypeByKeyQueryHandler(ILocationReadStore<RegionTypeResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<RegionTypeResponse>> Handle(
        GetRegionTypeByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.Code }, cancellationToken);

        if (item is null)
        {
            return Result<RegionTypeResponse>.Fail(
                "RegionType not found.",
                HttpStatusCode.NotFound);
        }

        return Result<RegionTypeResponse>.Ok(item);
    }
}
