namespace LocationService.Application.Feature.LocalCommunityCoverage.Command.Create;

public sealed class CreateLocalCommunityCoverageCommandHandler
    : ICommandHandler<CreateLocalCommunityCoverageCommand, Result<LocalCommunityCoverageResponse>>
{
    private readonly ILocationReadStore<LocalCommunityCoverageResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateLocalCommunityCoverageCommandHandler(
        ILocationReadStore<LocalCommunityCoverageResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<LocalCommunityCoverageResponse>> Handle(
        CreateLocalCommunityCoverageCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.CountryCode, request.MunicipalityCode, request.LocalCommunityIdentifier, request.SettlementCode }, cancellationToken))
        {
            return Result<LocalCommunityCoverageResponse>.Fail(
                "LocalCommunityCoverage already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.LocalCommunityCoverage.Create(request.CountryCode, request.MunicipalityCode, request.LocalCommunityIdentifier, request.SettlementCode);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LocalCommunityCoverageResponse>.Created(entity.ToResponse());
    }
}
