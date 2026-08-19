namespace LocationService.Application.Feature.HealthInstitutionType.Command.Create;

public sealed class CreateHealthInstitutionTypeCommandHandler
    : ICommandHandler<CreateHealthInstitutionTypeCommand, Result<HealthInstitutionTypeResponse>>
{
    private readonly ILocationReadStore<HealthInstitutionTypeResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateHealthInstitutionTypeCommandHandler(
        ILocationReadStore<HealthInstitutionTypeResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<HealthInstitutionTypeResponse>> Handle(
        CreateHealthInstitutionTypeCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.Code }, cancellationToken))
        {
            return Result<HealthInstitutionTypeResponse>.Fail(
                "HealthInstitutionType already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.HealthInstitutionType.Create(request.Code, request.Name, request.Description);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<HealthInstitutionTypeResponse>.Created(entity.ToResponse());
    }
}
