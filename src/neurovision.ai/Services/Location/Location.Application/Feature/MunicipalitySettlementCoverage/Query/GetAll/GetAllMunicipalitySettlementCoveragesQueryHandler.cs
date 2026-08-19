namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Query.GetAll;

public sealed class GetAllMunicipalitySettlementCoveragesQueryHandler
    : IQueryHandler<GetAllMunicipalitySettlementCoveragesQuery, Result<PaginatedResult<MunicipalitySettlementCoverageResponse>>>
{
    private readonly ILocationReadStore<MunicipalitySettlementCoverageResponse> reads;

    public GetAllMunicipalitySettlementCoveragesQueryHandler(ILocationReadStore<MunicipalitySettlementCoverageResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<PaginatedResult<MunicipalitySettlementCoverageResponse>>> Handle(
        GetAllMunicipalitySettlementCoveragesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);
        var total = await reads.CountAsync(cancellationToken: cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.PageSize, Offset = request.PageIndex * request.PageSize },
            cancellationToken);

        return Result<PaginatedResult<MunicipalitySettlementCoverageResponse>>.Ok(
            new PaginatedResult<MunicipalitySettlementCoverageResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
