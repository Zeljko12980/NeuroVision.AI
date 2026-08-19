namespace LocationService.Application.Feature.HealthInstitution.Command.Delete;

public sealed class DeleteHealthInstitutionCommandHandler
    : ICommandHandler<DeleteHealthInstitutionCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteHealthInstitutionCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteHealthInstitutionCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.HealthInstitution>(
            new object[] { command.Id },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "HealthInstitution not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
