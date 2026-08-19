namespace LocationService.Application.Feature.HealthInstitutionType.Command.Update;

public sealed class UpdateHealthInstitutionTypeCommandHandler
    : ICommandHandler<UpdateHealthInstitutionTypeCommand, Result<HealthInstitutionTypeResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateHealthInstitutionTypeCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<HealthInstitutionTypeResponse>> Handle(
        UpdateHealthInstitutionTypeCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.HealthInstitutionType>(
            new object[] { command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<HealthInstitutionTypeResponse>.Fail(
                "HealthInstitutionType not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.Name, request.Description);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<HealthInstitutionTypeResponse>.Ok(entity.ToResponse());
    }
}
