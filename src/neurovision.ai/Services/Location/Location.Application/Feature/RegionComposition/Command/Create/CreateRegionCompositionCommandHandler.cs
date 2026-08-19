namespace LocationService.Application.Feature.RegionComposition.Command.Create;

public sealed class CreateRegionCompositionCommandHandler
    : ICommandHandler<CreateRegionCompositionCommand, Result<RegionCompositionResponse>>
{
    private readonly ILocationReadStore<RegionCompositionResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateRegionCompositionCommandHandler(
        ILocationReadStore<RegionCompositionResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<RegionCompositionResponse>> Handle(
        CreateRegionCompositionCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.ParentRegionTypeCode, request.ParentRegionCode, request.MemberRegionTypeCode, request.MemberRegionCode }, cancellationToken))
        {
            return Result<RegionCompositionResponse>.Fail(
                "RegionComposition already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.RegionComposition.Create(request.ParentRegionTypeCode, request.ParentRegionCode, request.MemberRegionTypeCode, request.MemberRegionCode);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RegionCompositionResponse>.Created(entity.ToResponse());
    }
}
