namespace LocationService.Application.Feature.Region.Command.Delete;

public sealed class DeleteRegionCommandHandler
    : ICommandHandler<DeleteRegionCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteRegionCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteRegionCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Region>(
            new object[] { command.TypeCode, command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "Region not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
