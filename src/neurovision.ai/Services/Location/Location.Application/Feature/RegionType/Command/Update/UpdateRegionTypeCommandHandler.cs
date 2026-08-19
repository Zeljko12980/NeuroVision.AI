namespace LocationService.Application.Feature.RegionType.Command.Update;

public sealed class UpdateRegionTypeCommandHandler
    : ICommandHandler<UpdateRegionTypeCommand, Result<RegionTypeResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateRegionTypeCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<RegionTypeResponse>> Handle(
        UpdateRegionTypeCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.RegionType>(
            new object[] { command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<RegionTypeResponse>.Fail(
                "RegionType not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.Name, request.Description);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RegionTypeResponse>.Ok(entity.ToResponse());
    }
}
