namespace PatientService.Application.Feature.RelationshipType.Command.Create;

public sealed record CreateRelationshipTypeCommand(CreateRelationshipTypeRequest Request) : ICommand<Result>;

public sealed class CreateRelationshipTypeCommandHandler : ICommandHandler<CreateRelationshipTypeCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateRelationshipTypeCommandHandler> logger;

    public CreateRelationshipTypeCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateRelationshipTypeCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateRelationshipTypeCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::PatientService.Domain.Entities.RelationshipType.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.RelationshipType>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("RelationshipType already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("RelationshipType created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
