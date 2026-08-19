namespace LocationService.Application.Feature.LocalCommunity.Command.Delete;

public sealed class DeleteLocalCommunityCommandHandler
    : ICommandHandler<DeleteLocalCommunityCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteLocalCommunityCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteLocalCommunityCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.LocalCommunity>(
            new object[] { command.CountryCode, command.MunicipalityCode, command.Identifier },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "LocalCommunity not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
