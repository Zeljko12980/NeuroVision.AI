namespace LocationService.Application.Feature.Capital.Command.Update;

public sealed class UpdateCapitalCommandHandler
    : ICommandHandler<UpdateCapitalCommand, Result<CapitalResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateCapitalCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<CapitalResponse>> Handle(
        UpdateCapitalCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Capital>(
            new object[] { command.CountryCode, command.SettlementCode, command.SequenceNumber },
            cancellationToken);

        if (entity is null)
        {
            return Result<CapitalResponse>.Fail(
                "Capital not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.From, request.To);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CapitalResponse>.Ok(entity.ToResponse());
    }
}
