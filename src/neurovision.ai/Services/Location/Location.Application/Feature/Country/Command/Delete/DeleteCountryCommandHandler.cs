namespace LocationService.Application.Feature.Country.Command.Delete;

public sealed class DeleteCountryCommandHandler
    : ICommandHandler<DeleteCountryCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteCountryCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteCountryCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Country>(
            new object[] { command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "Country not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
