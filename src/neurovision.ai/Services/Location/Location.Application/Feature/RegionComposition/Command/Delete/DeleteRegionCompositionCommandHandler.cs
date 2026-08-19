namespace LocationService.Application.Feature.RegionComposition.Command.Delete;

public sealed class DeleteRegionCompositionCommandHandler
    : ICommandHandler<DeleteRegionCompositionCommand, Result<bool>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public DeleteRegionCompositionCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteRegionCompositionCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.RegionComposition>(
            new object[] { command.ParentRegionTypeCode, command.ParentRegionCode, command.MemberRegionTypeCode, command.MemberRegionCode },
            cancellationToken);

        if (entity is null)
        {
            return Result<bool>.Fail(
                "RegionComposition not found.",
                HttpStatusCode.NotFound);
        }

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
