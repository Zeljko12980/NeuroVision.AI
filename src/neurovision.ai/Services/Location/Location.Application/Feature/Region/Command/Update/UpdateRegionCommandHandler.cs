namespace LocationService.Application.Feature.Region.Command.Update;

public sealed class UpdateRegionCommandHandler
    : ICommandHandler<UpdateRegionCommand, Result<RegionResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateRegionCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<RegionResponse>> Handle(
        UpdateRegionCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Region>(
            new object[] { command.TypeCode, command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<RegionResponse>.Fail(
                "Region not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.Name, request.BelongsToCountryCode, request.HeadquartersCountryCode, request.AdministrativeSeatSettlementCode);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RegionResponse>.Ok(entity.ToResponse());
    }
}
