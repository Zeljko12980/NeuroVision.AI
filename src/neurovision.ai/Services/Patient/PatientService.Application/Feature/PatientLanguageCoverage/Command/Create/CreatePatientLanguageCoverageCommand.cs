namespace PatientService.Application.Feature.PatientLanguageCoverage.Command.Create;

public sealed record CreatePatientLanguageCoverageCommand(CreatePatientLanguageCoverageRequest Request) : ICommand<Result>;

public sealed class CreatePatientLanguageCoverageCommandHandler : ICommandHandler<CreatePatientLanguageCoverageCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreatePatientLanguageCoverageCommandHandler> logger;

    public CreatePatientLanguageCoverageCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientLanguageCoverageCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreatePatientLanguageCoverageCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.PatientLanguageCoverage>([request.PatientId, request.LanguageCode], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("PatientLanguageCoverage already exists.", HttpStatusCode.Conflict);
            }

            var entity = global::PatientService.Domain.Entities.PatientLanguageCoverage.Create(request.PatientId, request.LanguageCode);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("PatientLanguageCoverage created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
