namespace LocationService.Application.Feature.Settlement.Command.Delete;

public sealed class DeleteSettlementCommandHandler
    : ICommandHandler<DeleteSettlementCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteSettlementCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteSettlementCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Settlement>(
            new object[] { command.CountryCode, command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "Settlement not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
