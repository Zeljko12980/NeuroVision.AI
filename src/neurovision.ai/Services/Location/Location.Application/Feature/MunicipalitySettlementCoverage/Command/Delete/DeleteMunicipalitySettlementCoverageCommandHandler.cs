namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Command.Delete;

public sealed class DeleteMunicipalitySettlementCoverageCommandHandler
    : ICommandHandler<DeleteMunicipalitySettlementCoverageCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteMunicipalitySettlementCoverageCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteMunicipalitySettlementCoverageCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.MunicipalitySettlementCoverage>(
            new object[] { command.MunicipalityCode, command.CountryCode, command.SettlementCode },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "MunicipalitySettlementCoverage not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
