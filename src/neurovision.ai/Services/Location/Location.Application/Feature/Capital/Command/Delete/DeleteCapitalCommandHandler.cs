namespace LocationService.Application.Feature.Capital.Command.Delete;

public sealed class DeleteCapitalCommandHandler
    : ICommandHandler<DeleteCapitalCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteCapitalCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteCapitalCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Capital>(
            new object[] { command.CountryCode, command.SettlementCode, command.SequenceNumber },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "Capital not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
