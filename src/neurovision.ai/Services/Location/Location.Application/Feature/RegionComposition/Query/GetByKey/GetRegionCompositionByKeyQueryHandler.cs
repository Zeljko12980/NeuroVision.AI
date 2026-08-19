namespace LocationService.Application.Feature.RegionComposition.Query.GetByKey;

public sealed class GetRegionCompositionByKeyQueryHandler
    : IQueryHandler<GetRegionCompositionByKeyQuery, Result<RegionCompositionResponse>>
{
    private readonly ILocationReadStore<RegionCompositionResponse> reads;

    public GetRegionCompositionByKeyQueryHandler(ILocationReadStore<RegionCompositionResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<RegionCompositionResponse>> Handle(
        GetRegionCompositionByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.ParentRegionTypeCode, query.ParentRegionCode, query.MemberRegionTypeCode, query.MemberRegionCode }, cancellationToken);

        if (item is null)
        {
            return Result<RegionCompositionResponse>.Fail(
                "RegionComposition not found.",
                HttpStatusCode.NotFound);
        }

        return Result<RegionCompositionResponse>.Ok(item);
    }
}
