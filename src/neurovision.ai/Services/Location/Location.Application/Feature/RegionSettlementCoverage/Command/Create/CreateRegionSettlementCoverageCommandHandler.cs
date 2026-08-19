namespace LocationService.Application.Feature.RegionSettlementCoverage.Command.Create;

public sealed class CreateRegionSettlementCoverageCommandHandler
    : ICommandHandler<CreateRegionSettlementCoverageCommand, Result<RegionSettlementCoverageResponse>>
{
    private readonly ILocationReadStore<RegionSettlementCoverageResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateRegionSettlementCoverageCommandHandler(
        ILocationReadStore<RegionSettlementCoverageResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<RegionSettlementCoverageResponse>> Handle(
        CreateRegionSettlementCoverageCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.RegionTypeCode, request.RegionCode, request.CountryCode, request.SettlementCode }, cancellationToken))
        {
            return Result<RegionSettlementCoverageResponse>.Fail(
                "RegionSettlementCoverage already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.RegionSettlementCoverage.Create(request.RegionTypeCode, request.RegionCode, request.CountryCode, request.SettlementCode);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RegionSettlementCoverageResponse>.Created(entity.ToResponse());
    }
}
