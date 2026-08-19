namespace LocationService.Application.Feature.HealthInstitutionType.Command.Delete;

public sealed class DeleteHealthInstitutionTypeCommandHandler
    : ICommandHandler<DeleteHealthInstitutionTypeCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteHealthInstitutionTypeCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteHealthInstitutionTypeCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.HealthInstitutionType>(
            new object[] { command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "HealthInstitutionType not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
