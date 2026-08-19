namespace LocationService.Application.Feature.GovernmentType.Command.Delete;

public sealed class DeleteGovernmentTypeCommandHandler
    : ICommandHandler<DeleteGovernmentTypeCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteGovernmentTypeCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteGovernmentTypeCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.GovernmentType>(
            new object[] { command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "GovernmentType not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
