namespace LocationService.Application.Feature.GovernmentHistory.Command.Create;

public sealed class CreateGovernmentHistoryCommandHandler
    : ICommandHandler<CreateGovernmentHistoryCommand, Result<GovernmentHistoryResponse>>
{
    private readonly ILocationReadStore<GovernmentHistoryResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateGovernmentHistoryCommandHandler(
        ILocationReadStore<GovernmentHistoryResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<GovernmentHistoryResponse>> Handle(
        CreateGovernmentHistoryCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.CountryCode, request.SequenceNumber }, cancellationToken))
        {
            return Result<GovernmentHistoryResponse>.Fail(
                "GovernmentHistory already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.GovernmentHistory.Create(request.CountryCode, request.SequenceNumber, request.GovernmentTypeCode, request.From, request.To);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<GovernmentHistoryResponse>.Created(entity.ToResponse());
    }
}
