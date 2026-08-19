namespace LocationService.Application.Feature.Capital.Query.GetByKey;

public sealed class GetCapitalByKeyQueryHandler
    : IQueryHandler<GetCapitalByKeyQuery, Result<CapitalResponse>>
{
    private readonly ILocationReadStore<CapitalResponse> reads;

    public GetCapitalByKeyQueryHandler(ILocationReadStore<CapitalResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<CapitalResponse>> Handle(
        GetCapitalByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.CountryCode, query.SettlementCode, query.SequenceNumber }, cancellationToken);

        if (item is null)
        {
            return Result<CapitalResponse>.Fail(
                "Capital not found.",
                HttpStatusCode.NotFound);
        }

        return Result<CapitalResponse>.Ok(item);
    }
}
