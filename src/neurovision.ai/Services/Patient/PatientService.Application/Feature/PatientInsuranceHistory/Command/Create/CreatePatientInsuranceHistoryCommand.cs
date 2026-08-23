namespace PatientService.Application.Feature.PatientInsuranceHistory.Command.Create;

public sealed record CreatePatientInsuranceHistoryCommand(CreatePatientInsuranceHistoryRequest Request) : ICommand<Result>;

public sealed class CreatePatientInsuranceHistoryCommandHandler : ICommandHandler<CreatePatientInsuranceHistoryCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreatePatientInsuranceHistoryCommandHandler> logger;

    public CreatePatientInsuranceHistoryCommandHandler(
        IPatientWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientInsuranceHistoryCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreatePatientInsuranceHistoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "PatientInsuranceHistories",
                "SequenceNumber",
                cancellationToken,
                ("PatientId", request.PatientId));
            var entity = global::PatientService.Domain.Entities.PatientInsuranceHistory.Create(request.PatientId, sequence, request.PayerCode, request.PolicyNumber, request.From, request.To);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("PatientInsuranceHistory created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
