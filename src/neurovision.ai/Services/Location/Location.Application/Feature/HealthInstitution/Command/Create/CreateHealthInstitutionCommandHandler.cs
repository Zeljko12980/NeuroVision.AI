namespace LocationService.Application.Feature.HealthInstitution.Command.Create;

public sealed class CreateHealthInstitutionCommandHandler
    : ICommandHandler<CreateHealthInstitutionCommand, Result<HealthInstitutionResponse>>
{
    private readonly ILocationReadStore<HealthInstitutionResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateHealthInstitutionCommandHandler(
        ILocationReadStore<HealthInstitutionResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<HealthInstitutionResponse>> Handle(
        CreateHealthInstitutionCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        var entity = global::LocationService.Domain.Entities.HealthInstitution.Create(request.Name, request.TypeCode, request.CountryCode, request.SettlementCode, request.Address, request.BedCount, request.FoundingDate, request.Phone);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<HealthInstitutionResponse>.Created(entity.ToResponse());
    }
}
