namespace LocationService.Application.Feature.HealthInstitution.Command.Update;

public sealed class UpdateHealthInstitutionCommandHandler
    : ICommandHandler<UpdateHealthInstitutionCommand, Result<HealthInstitutionResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateHealthInstitutionCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<HealthInstitutionResponse>> Handle(
        UpdateHealthInstitutionCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.HealthInstitution>(
            new object[] { command.Id },
            cancellationToken);

        if (entity is null)
        {
            return Result<HealthInstitutionResponse>.Fail(
                "HealthInstitution not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.Name, request.TypeCode, request.CountryCode, request.SettlementCode, request.Address, request.BedCount, request.FoundingDate, request.Phone);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<HealthInstitutionResponse>.Ok(entity.ToResponse());
    }
}
