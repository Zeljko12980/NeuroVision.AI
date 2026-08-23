namespace PatientService.Application.Feature.PatientConditionCoverage.Command.Create;

public sealed record CreatePatientConditionCoverageCommand(CreatePatientConditionCoverageRequest Request) : ICommand<Result>;

public sealed class CreatePatientConditionCoverageCommandHandler : ICommandHandler<CreatePatientConditionCoverageCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreatePatientConditionCoverageCommandHandler> logger;

    public CreatePatientConditionCoverageCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientConditionCoverageCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreatePatientConditionCoverageCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.PatientConditionCoverage>([request.PatientId, request.ConditionCode], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("PatientConditionCoverage already exists.", HttpStatusCode.Conflict);
            }

            var entity = global::PatientService.Domain.Entities.PatientConditionCoverage.Create(request.PatientId, request.ConditionCode, request.DiagnosedYear, request.Note);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("PatientConditionCoverage created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
