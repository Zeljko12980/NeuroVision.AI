namespace LocationService.Application.Feature.GovernmentHistory.Command.Delete;

public sealed class DeleteGovernmentHistoryCommandHandler
    : ICommandHandler<DeleteGovernmentHistoryCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteGovernmentHistoryCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteGovernmentHistoryCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.GovernmentHistory>(
            new object[] { command.CountryCode, command.SequenceNumber },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "GovernmentHistory not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
