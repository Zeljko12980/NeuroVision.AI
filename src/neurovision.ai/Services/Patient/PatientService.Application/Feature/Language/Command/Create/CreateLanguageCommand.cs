namespace PatientService.Application.Feature.Language.Command.Create;

public sealed record CreateLanguageCommand(CreateLanguageRequest Request) : ICommand<Result>;

public sealed class CreateLanguageCommandHandler : ICommandHandler<CreateLanguageCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateLanguageCommandHandler> logger;

    public CreateLanguageCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateLanguageCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateLanguageCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::PatientService.Domain.Entities.Language.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.Language>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("Language already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Language created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
