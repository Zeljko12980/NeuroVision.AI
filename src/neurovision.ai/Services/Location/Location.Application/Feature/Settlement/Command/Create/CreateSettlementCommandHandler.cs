namespace LocationService.Application.Feature.Settlement.Command.Create;

public sealed class CreateSettlementCommandHandler
    : ICommandHandler<CreateSettlementCommand, Result<SettlementResponse>>
{
    private readonly ILocationReadStore<SettlementResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateSettlementCommandHandler(
        ILocationReadStore<SettlementResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<SettlementResponse>> Handle(
        CreateSettlementCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.CountryCode, request.Code }, cancellationToken))
        {
            return Result<SettlementResponse>.Fail(
                "Settlement already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.Settlement.Create(request.CountryCode, request.Code, request.Name, request.PostalCode);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SettlementResponse>.Created(entity.ToResponse());
    }
}
