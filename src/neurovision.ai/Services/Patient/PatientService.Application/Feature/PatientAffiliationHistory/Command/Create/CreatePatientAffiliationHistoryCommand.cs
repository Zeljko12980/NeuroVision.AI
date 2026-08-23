namespace PatientService.Application.Feature.PatientAffiliationHistory.Command.Create;

public sealed record CreatePatientAffiliationHistoryCommand(CreatePatientAffiliationHistoryRequest Request) : ICommand<Result>;

public sealed class CreatePatientAffiliationHistoryCommandHandler : ICommandHandler<CreatePatientAffiliationHistoryCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreatePatientAffiliationHistoryCommandHandler> logger;

    public CreatePatientAffiliationHistoryCommandHandler(
        IPatientWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientAffiliationHistoryCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreatePatientAffiliationHistoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "PatientAffiliationHistories",
                "SequenceNumber",
                cancellationToken,
                ("PatientId", request.PatientId));
            var entity = global::PatientService.Domain.Entities.PatientAffiliationHistory.Create(request.PatientId, sequence, request.InstitutionName, request.HealthInstitutionId, request.From, request.To);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("PatientAffiliationHistory created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
