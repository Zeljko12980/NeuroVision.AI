namespace LocationService.Application.Feature.GovernmentHistory.Command.Update;

public sealed class UpdateGovernmentHistoryCommandHandler
    : ICommandHandler<UpdateGovernmentHistoryCommand, Result<GovernmentHistoryResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateGovernmentHistoryCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<GovernmentHistoryResponse>> Handle(
        UpdateGovernmentHistoryCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.GovernmentHistory>(
            new object[] { command.CountryCode, command.SequenceNumber },
            cancellationToken);

        if (entity is null)
        {
            return Result<GovernmentHistoryResponse>.Fail(
                "GovernmentHistory not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.GovernmentTypeCode, request.From, request.To);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<GovernmentHistoryResponse>.Ok(entity.ToResponse());
    }
}
