namespace LocationService.Application.Feature.RegionSettlementCoverage.Command.Delete;

public sealed class DeleteRegionSettlementCoverageCommandHandler
    : ICommandHandler<DeleteRegionSettlementCoverageCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteRegionSettlementCoverageCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteRegionSettlementCoverageCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.RegionSettlementCoverage>(
            new object[] { command.CountryCode, command.SettlementCode, command.RegionTypeCode, command.RegionCode },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "RegionSettlementCoverage not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
