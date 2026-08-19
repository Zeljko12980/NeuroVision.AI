namespace LocationService.Application.Feature.Country.Command.Update;

public sealed class UpdateCountryCommandHandler
    : ICommandHandler<UpdateCountryCommand, Result<CountryResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateCountryCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<CountryResponse>> Handle(
        UpdateCountryCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Country>(
            new object[] { command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<CountryResponse>.Fail(
                "Country not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.Name, request.FoundingDate, request.CapitalSettlementCode, request.GovernmentTypeCode, request.CallingCode, request.Anthem, request.CoatOfArms, request.Flag);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CountryResponse>.Ok(entity.ToResponse());
    }
}
