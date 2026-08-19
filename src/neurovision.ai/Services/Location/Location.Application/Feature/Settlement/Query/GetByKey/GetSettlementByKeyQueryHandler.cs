namespace LocationService.Application.Feature.Settlement.Query.GetByKey;

public sealed class GetSettlementByKeyQueryHandler
    : IQueryHandler<GetSettlementByKeyQuery, Result<SettlementResponse>>
{
    private readonly ILocationReadStore<SettlementResponse> reads;

    public GetSettlementByKeyQueryHandler(ILocationReadStore<SettlementResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<SettlementResponse>> Handle(
        GetSettlementByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.CountryCode, query.Code }, cancellationToken);

        if (item is null)
        {
            return Result<SettlementResponse>.Fail(
                "Settlement not found.",
                HttpStatusCode.NotFound);
        }

        return Result<SettlementResponse>.Ok(item);
    }
}
