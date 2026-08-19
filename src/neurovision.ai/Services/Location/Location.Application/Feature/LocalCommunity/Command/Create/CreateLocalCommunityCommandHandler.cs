namespace LocationService.Application.Feature.LocalCommunity.Command.Create;

public sealed class CreateLocalCommunityCommandHandler
    : ICommandHandler<CreateLocalCommunityCommand, Result<LocalCommunityResponse>>
{
    private readonly ILocationReadStore<LocalCommunityResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateLocalCommunityCommandHandler(
        ILocationReadStore<LocalCommunityResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<LocalCommunityResponse>> Handle(
        CreateLocalCommunityCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.CountryCode, request.MunicipalityCode, request.Identifier }, cancellationToken))
        {
            return Result<LocalCommunityResponse>.Fail(
                "LocalCommunity already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.LocalCommunity.Create(request.CountryCode, request.MunicipalityCode, request.Identifier, request.Name, request.OfficeSettlementCode);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LocalCommunityResponse>.Created(entity.ToResponse());
    }
}
