namespace LocationService.Application.Feature.Capital.Command.Create;

public sealed class CreateCapitalCommandHandler
    : ICommandHandler<CreateCapitalCommand, Result<CapitalResponse>>
{
    private readonly ILocationReadStore<CapitalResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateCapitalCommandHandler(
        ILocationReadStore<CapitalResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<CapitalResponse>> Handle(
        CreateCapitalCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.CountryCode, request.SettlementCode, request.SequenceNumber }, cancellationToken))
        {
            return Result<CapitalResponse>.Fail(
                "Capital already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.Capital.Create(request.CountryCode, request.SettlementCode, request.SequenceNumber, request.From, request.To);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CapitalResponse>.Created(entity.ToResponse());
    }
}
