namespace LocationService.Application.Feature.GovernmentType.Query.GetByKey;

public sealed class GetGovernmentTypeByKeyQueryHandler
    : IQueryHandler<GetGovernmentTypeByKeyQuery, Result<GovernmentTypeResponse>>
{
    private readonly ILocationReadStore<GovernmentTypeResponse> reads;

    public GetGovernmentTypeByKeyQueryHandler(ILocationReadStore<GovernmentTypeResponse> reads)
    {
        this.reads = reads;
    }

    public async Task<Result<GovernmentTypeResponse>> Handle(
        GetGovernmentTypeByKeyQuery query,
        CancellationToken cancellationToken)
    {
        var item = await reads.GetByKeyAsync(new { query.Code }, cancellationToken);

        if (item is null)
        {
            return Result<GovernmentTypeResponse>.Fail(
                "GovernmentType not found.",
                HttpStatusCode.NotFound);
        }

        return Result<GovernmentTypeResponse>.Ok(item);
    }
}
