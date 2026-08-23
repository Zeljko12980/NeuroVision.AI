namespace PatientService.Application.Feature.ConsentType.Command.Create;

public sealed record CreateConsentTypeCommand(CreateConsentTypeRequest Request) : ICommand<Result>;

public sealed class CreateConsentTypeCommandHandler : ICommandHandler<CreateConsentTypeCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateConsentTypeCommandHandler> logger;

    public CreateConsentTypeCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateConsentTypeCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateConsentTypeCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::PatientService.Domain.Entities.ConsentType.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.ConsentType>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("ConsentType already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("ConsentType created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
