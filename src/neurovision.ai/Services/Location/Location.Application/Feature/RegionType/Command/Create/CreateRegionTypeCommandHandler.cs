namespace LocationService.Application.Feature.RegionType.Command.Create;

public sealed class CreateRegionTypeCommandHandler
    : ICommandHandler<CreateRegionTypeCommand, Result<RegionTypeResponse>>
{
    private readonly ILocationReadStore<RegionTypeResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateRegionTypeCommandHandler(
        ILocationReadStore<RegionTypeResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<RegionTypeResponse>> Handle(
        CreateRegionTypeCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.Code }, cancellationToken))
        {
            return Result<RegionTypeResponse>.Fail(
                "RegionType already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.RegionType.Create(request.Code, request.Name, request.Description);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RegionTypeResponse>.Created(entity.ToResponse());
    }
}
