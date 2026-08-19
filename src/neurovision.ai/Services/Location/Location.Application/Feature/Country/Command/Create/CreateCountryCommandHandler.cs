namespace LocationService.Application.Feature.Country.Command.Create;

public sealed class CreateCountryCommandHandler
    : ICommandHandler<CreateCountryCommand, Result<CountryResponse>>
{
    private readonly ILocationReadStore<CountryResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateCountryCommandHandler(
        ILocationReadStore<CountryResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<CountryResponse>> Handle(
        CreateCountryCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.Code }, cancellationToken))
        {
            return Result<CountryResponse>.Fail(
                "Country already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.Country.Create(request.Code, request.Name, request.FoundingDate, request.CapitalSettlementCode, request.GovernmentTypeCode, request.CallingCode, request.Anthem, request.CoatOfArms, request.Flag);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CountryResponse>.Created(entity.ToResponse());
    }
}
