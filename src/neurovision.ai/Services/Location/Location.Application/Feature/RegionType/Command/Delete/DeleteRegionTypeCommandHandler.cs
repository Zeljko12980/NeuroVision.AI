namespace LocationService.Application.Feature.RegionType.Command.Delete;

public sealed class DeleteRegionTypeCommandHandler
    : ICommandHandler<DeleteRegionTypeCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteRegionTypeCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteRegionTypeCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.RegionType>(
            new object[] { command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "RegionType not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
