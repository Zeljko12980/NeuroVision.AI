namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Command.Create;

public sealed class CreateMunicipalitySettlementCoverageCommandHandler
    : ICommandHandler<CreateMunicipalitySettlementCoverageCommand, Result<MunicipalitySettlementCoverageResponse>>
{
    private readonly ILocationReadStore<MunicipalitySettlementCoverageResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateMunicipalitySettlementCoverageCommandHandler(
        ILocationReadStore<MunicipalitySettlementCoverageResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<MunicipalitySettlementCoverageResponse>> Handle(
        CreateMunicipalitySettlementCoverageCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.CountryCode, request.MunicipalityCode, request.SettlementCode }, cancellationToken))
        {
            return Result<MunicipalitySettlementCoverageResponse>.Fail(
                "MunicipalitySettlementCoverage already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.MunicipalitySettlementCoverage.Create(request.CountryCode, request.MunicipalityCode, request.SettlementCode);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MunicipalitySettlementCoverageResponse>.Created(entity.ToResponse());
    }
}
