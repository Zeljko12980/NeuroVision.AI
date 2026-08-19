namespace LocationService.Application.Feature.CountryComposition.Command.Update;

public sealed class UpdateCountryCompositionCommandHandler
    : ICommandHandler<UpdateCountryCompositionCommand, Result<CountryCompositionResponse>>
{
    private readonly ILocationWriteStore writes;
    private readonly IUnitOfWork unitOfWork;

    public UpdateCountryCompositionCommandHandler(
        ILocationWriteStore writes,
        IUnitOfWork unitOfWork)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<CountryCompositionResponse>> Handle(
        UpdateCountryCompositionCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await writes.FindAsync<global::LocationService.Domain.Entities.CountryComposition>(
            new object[] { command.MemberCountryCode, command.UnionCountryCode, command.SequenceNumber },
            cancellationToken);

        if (entity is null)
        {
            return Result<CountryCompositionResponse>.Fail(
                "CountryComposition not found.",
                HttpStatusCode.NotFound);
        }

        var request = command.Request;
        entity.Update(request.From, request.To);
        writes.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CountryCompositionResponse>.Ok(entity.ToResponse());
    }
}
