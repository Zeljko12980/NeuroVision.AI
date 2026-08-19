namespace LocationService.Application.Feature.LocalCommunityCoverage.Command.Delete;

public sealed class DeleteLocalCommunityCoverageCommandHandler
    : ICommandHandler<DeleteLocalCommunityCoverageCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteLocalCommunityCoverageCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteLocalCommunityCoverageCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.LocalCommunityCoverage>(
            new object[] { command.MunicipalityCode, command.LocalCommunityIdentifier, command.CountryCode, command.SettlementCode },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "LocalCommunityCoverage not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
