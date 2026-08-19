namespace LocationService.Application.Feature.Municipality.Command.Delete;

public sealed class DeleteMunicipalityCommandHandler
    : ICommandHandler<DeleteMunicipalityCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteMunicipalityCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteMunicipalityCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Municipality>(
            new object[] { command.CountryCode, command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "Municipality not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
