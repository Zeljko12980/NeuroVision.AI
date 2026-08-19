namespace LocationService.Application.Feature.LegalSuccessor.Command.Delete;

public sealed class DeleteLegalSuccessorCommandHandler
    : ICommandHandler<DeleteLegalSuccessorCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteLegalSuccessorCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteLegalSuccessorCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.LegalSuccessor>(
            new object[] { command.PredecessorCountryCode, command.SuccessorCountryCode },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "LegalSuccessor not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
