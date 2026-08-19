namespace LocationService.Application.Feature.GovernmentType.Command.Update;

public sealed class UpdateGovernmentTypeCommandHandler
    : ICommandHandler<UpdateGovernmentTypeCommand, Result<GovernmentTypeResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateGovernmentTypeCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<GovernmentTypeResponse>> Handle(
        UpdateGovernmentTypeCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.GovernmentType>(
            new object[] { command.Code },
            cancellationToken);

        if (entity is null)
        {
            return Result<GovernmentTypeResponse>.Fail(
                "GovernmentType not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.Name, request.Description);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<GovernmentTypeResponse>.Ok(entity.ToResponse());
    }
}
