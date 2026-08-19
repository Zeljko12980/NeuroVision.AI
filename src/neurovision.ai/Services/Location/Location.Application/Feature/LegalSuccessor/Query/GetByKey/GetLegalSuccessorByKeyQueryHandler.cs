namespace LocationService.Application.Feature.LegalSuccessor.Query.GetByKey;

public sealed class GetLegalSuccessorByKeyQueryHandler
    : IQueryHandler<GetLegalSuccessorByKeyQuery, Result<LegalSuccessorResponse>>
{
    private readonly ILocationReadStore<LegalSuccessorResponse> reads;

    public GetLegalSuccessorByKeyQueryHandler(ILocationReadStore<LegalSuccessorResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<LegalSuccessorResponse>> Handle(
        GetLegalSuccessorByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.SuccessorCountryCode, query.PredecessorCountryCode }, cancellationToken);

        if (item is null)
        {
            return Result<LegalSuccessorResponse>.Fail(
                "LegalSuccessor not found.",
                HttpStatusCode.NotFound);
        }

        return Result<LegalSuccessorResponse>.Ok(item);
    }
}
