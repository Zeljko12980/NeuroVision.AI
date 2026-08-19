namespace LocationService.Application.Feature.Settlement.Command.Update;

public sealed class UpdateSettlementCommandHandler
    : ICommandHandler<UpdateSettlementCommand, Result<SettlementResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateSettlementCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<SettlementResponse>> Handle(
        UpdateSettlementCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.Settlement>(
            new object[] { command.CountryCode, command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<SettlementResponse>.Fail(
                "Settlement not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.Name, request.PostalCode);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SettlementResponse>.Ok(entity.ToResponse());
    }
}
