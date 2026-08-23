namespace PatientService.Application.Feature.PatientConsentCoverage.Command.Create;

public sealed record CreatePatientConsentCoverageCommand(CreatePatientConsentCoverageRequest Request) : ICommand<Result>;

public sealed class CreatePatientConsentCoverageCommandHandler : ICommandHandler<CreatePatientConsentCoverageCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreatePatientConsentCoverageCommandHandler> logger;

    public CreatePatientConsentCoverageCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientConsentCoverageCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreatePatientConsentCoverageCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.PatientConsentCoverage>([request.PatientId, request.ConsentTypeCode], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("PatientConsentCoverage already exists.", HttpStatusCode.Conflict);
            }

            var entity = global::PatientService.Domain.Entities.PatientConsentCoverage.Create(request.PatientId, request.ConsentTypeCode, request.From, request.To);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("PatientConsentCoverage created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
