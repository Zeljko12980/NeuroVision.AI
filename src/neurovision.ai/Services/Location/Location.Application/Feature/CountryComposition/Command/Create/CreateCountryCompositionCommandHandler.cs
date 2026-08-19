namespace LocationService.Application.Feature.CountryComposition.Command.Create;

public sealed class CreateCountryCompositionCommandHandler
    : ICommandHandler<CreateCountryCompositionCommand, Result<CountryCompositionResponse>>
{
    private readonly ILocationReadStore<CountryCompositionResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateCountryCompositionCommandHandler(
        ILocationReadStore<CountryCompositionResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<CountryCompositionResponse>> Handle(
        CreateCountryCompositionCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.UnionCountryCode, request.MemberCountryCode, request.SequenceNumber }, cancellationToken))
        {
            return Result<CountryCompositionResponse>.Fail(
                "CountryComposition already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.CountryComposition.Create(request.UnionCountryCode, request.MemberCountryCode, request.SequenceNumber, request.From, request.To);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CountryCompositionResponse>.Created(entity.ToResponse());
    }
}
