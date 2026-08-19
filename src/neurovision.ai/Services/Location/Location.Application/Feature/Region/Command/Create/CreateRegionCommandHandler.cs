namespace LocationService.Application.Feature.Region.Command.Create;

public sealed class CreateRegionCommandHandler
    : ICommandHandler<CreateRegionCommand, Result<RegionResponse>>
{
    private readonly ILocationReadStore<RegionResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateRegionCommandHandler(
        ILocationReadStore<RegionResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<RegionResponse>> Handle(
        CreateRegionCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.TypeCode, request.Code }, cancellationToken))
        {
            return Result<RegionResponse>.Fail(
                "Region already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.Region.Create(request.TypeCode, request.Code, request.Name, request.BelongsToCountryCode, request.HeadquartersCountryCode, request.AdministrativeSeatSettlementCode);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RegionResponse>.Created(entity.ToResponse());
    }
}
