namespace PatientService.Application.Feature.RelationshipType.Query.GetAll;

public sealed class GetAllRelationshipTypesQueryHandler
    : IQueryHandler<GetAllRelationshipTypesQuery, Result<PaginatedResult<RelationshipTypeResponse>>>
{
    private readonly IPatientReadStore<RelationshipTypeResponse> reads;
    private readonly ILogger<GetAllRelationshipTypesQueryHandler> logger;

    public GetAllRelationshipTypesQueryHandler(
        IPatientReadStore<RelationshipTypeResponse> reads,
        ILogger<GetAllRelationshipTypesQueryHandler> logger)
    {
        this.reads = reads;
        this.logger = logger;
    }

    public async Task<Result<PaginatedResult<RelationshipTypeResponse>>> Handle(
        GetAllRelationshipTypesQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var pageIndex = Math.Max(request.PageIndex, 0);

        logger.LogInformation(
            "Get relationship types started. PageIndex={PageIndex}, PageSize={PageSize}, Search={Search}",
            pageIndex,
            request.PageSize,
            request.Search);

        var total = await reads.CountAsync(new { request.Search }, cancellationToken);
        var items = await reads.GetPagedAsync(
            new { request.Search, request.PageSize, Offset = pageIndex * request.PageSize },
            cancellationToken);

        logger.LogInformation("Get relationship types succeeded. Count={Count}", total);

        return Result<PaginatedResult<RelationshipTypeResponse>>.Ok(
            new PaginatedResult<RelationshipTypeResponse>(
                pageIndex,
                request.PageSize,
                total,
                items));
    }
}
