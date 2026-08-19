namespace LocationService.Application.Feature.LegalSuccessor.Command.Create;

public sealed class CreateLegalSuccessorCommandHandler
    : ICommandHandler<CreateLegalSuccessorCommand, Result<LegalSuccessorResponse>>
{
    private readonly ILocationReadStore<LegalSuccessorResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateLegalSuccessorCommandHandler(
        ILocationReadStore<LegalSuccessorResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalSuccessorResponse>> Handle(
        CreateLegalSuccessorCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.SuccessorCountryCode, request.PredecessorCountryCode }, cancellationToken))
        {
            return Result<LegalSuccessorResponse>.Fail(
                "LegalSuccessor already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.LegalSuccessor.Create(request.SuccessorCountryCode, request.PredecessorCountryCode);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LegalSuccessorResponse>.Created(entity.ToResponse());
    }
}
