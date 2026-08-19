namespace LocationService.Application.Feature.Municipality.Command.Update;

public sealed class UpdateMunicipalityCommandHandler
    : ICommandHandler<UpdateMunicipalityCommand, Result<MunicipalityResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateMunicipalityCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<MunicipalityResponse>> Handle(
        UpdateMunicipalityCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Municipality>(
            new object[] { command.CountryCode, command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<MunicipalityResponse>.Fail(
                "Municipality not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.Name, request.SeatSettlementCode);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MunicipalityResponse>.Ok(entity.ToResponse());
    }
}
