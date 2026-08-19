namespace LocationService.Application.Feature.CountryComposition.Command.Delete;

public sealed class DeleteCountryCompositionCommandHandler
    : ICommandHandler<DeleteCountryCompositionCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteCountryCompositionCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteCountryCompositionCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.CountryComposition>(
            new object[] { command.MemberCountryCode, command.UnionCountryCode, command.SequenceNumber },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "CountryComposition not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
