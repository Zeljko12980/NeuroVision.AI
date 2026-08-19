namespace LocationService.Application.Feature.LocalCommunity.Command.Update;

public sealed class UpdateLocalCommunityCommandHandler
    : ICommandHandler<UpdateLocalCommunityCommand, Result<LocalCommunityResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateLocalCommunityCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<LocalCommunityResponse>> Handle(
        UpdateLocalCommunityCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.LocalCommunity>(
            new object[] { command.CountryCode, command.MunicipalityCode, command.Identifier },
            cancellationToken);

        if (entity is null)
        {
            return Result<LocalCommunityResponse>.Fail(
                "LocalCommunity not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.Name, request.OfficeSettlementCode);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LocalCommunityResponse>.Ok(entity.ToResponse());
    }
}
