namespace LocationService.Application.Feature.GovernmentHistory.Query.GetByKey;

public sealed class GetGovernmentHistoryByKeyQueryHandler
    : IQueryHandler<GetGovernmentHistoryByKeyQuery, Result<GovernmentHistoryResponse>>
{
    private readonly ILocationReadStore<GovernmentHistoryResponse> reads;

    public GetGovernmentHistoryByKeyQueryHandler(ILocationReadStore<GovernmentHistoryResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<GovernmentHistoryResponse>> Handle(
        GetGovernmentHistoryByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.CountryCode, query.SequenceNumber }, cancellationToken);

        if (item is null)
        {
            return Result<GovernmentHistoryResponse>.Fail(
                "GovernmentHistory not found.",
                HttpStatusCode.NotFound);
        }

        return Result<GovernmentHistoryResponse>.Ok(item);
    }
}
