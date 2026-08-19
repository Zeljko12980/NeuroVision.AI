namespace LocationService.Application.Feature.GovernmentType.Command.Create;

public sealed class CreateGovernmentTypeCommandHandler
    : ICommandHandler<CreateGovernmentTypeCommand, Result<GovernmentTypeResponse>>
{
    private readonly ILocationReadStore<GovernmentTypeResponse> reads;
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public CreateGovernmentTypeCommandHandler(
        ILocationReadStore<GovernmentTypeResponse> reads,
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.reads = reads;
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<GovernmentTypeResponse>> Handle(
        CreateGovernmentTypeCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await reads.ExistsAsync(new { request.Code }, cancellationToken))
        {
            return Result<GovernmentTypeResponse>.Fail(
                "GovernmentType already exists.",
                HttpStatusCode.Conflict);
        }

        var entity = global::LocationService.Domain.Entities.GovernmentType.Create(request.Code, request.Name, request.Description);

        await writes.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<GovernmentTypeResponse>.Created(entity.ToResponse());
    }
}
