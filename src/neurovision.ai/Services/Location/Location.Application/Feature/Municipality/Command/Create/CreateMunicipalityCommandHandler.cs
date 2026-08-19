namespace LocationService.Application.Feature.Municipality.Command.Create;

public sealed class CreateMunicipalityCommandHandler
    : ICommandHandler<CreateMunicipalityCommand, Result<MunicipalityResponse>>
{
    private readonly ILocationReadStore<MunicipalityResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateMunicipalityCommandHandler(
        ILocationReadStore<MunicipalityResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<MunicipalityResponse>> Handle(
        CreateMunicipalityCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.CountryCode, request.Code }, cancellationToken))
        {
            return Result<MunicipalityResponse>.Fail(
                "Municipality already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.Municipality.Create(request.CountryCode, request.Code, request.Name, request.SeatSettlementCode);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MunicipalityResponse>.Created(entity.ToResponse());
    }
}
