namespace PatientService.Application.Feature.PatientAllergyCoverage.Command.Create;

public sealed record CreatePatientAllergyCoverageCommand(CreatePatientAllergyCoverageRequest Request) : ICommand<Result>;

public sealed class CreatePatientAllergyCoverageCommandHandler : ICommandHandler<CreatePatientAllergyCoverageCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreatePatientAllergyCoverageCommandHandler> logger;

    public CreatePatientAllergyCoverageCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientAllergyCoverageCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreatePatientAllergyCoverageCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.PatientAllergyCoverage>([request.PatientId, request.AllergyCode], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("PatientAllergyCoverage already exists.", HttpStatusCode.Conflict);
            }

            var entity = global::PatientService.Domain.Entities.PatientAllergyCoverage.Create(request.PatientId, request.AllergyCode, request.Note);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("PatientAllergyCoverage created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
