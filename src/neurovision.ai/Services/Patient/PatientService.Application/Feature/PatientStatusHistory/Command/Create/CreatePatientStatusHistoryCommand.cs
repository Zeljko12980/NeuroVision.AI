namespace PatientService.Application.Feature.PatientStatusHistory.Command.Create;

public sealed record CreatePatientStatusHistoryCommand(CreatePatientStatusHistoryRequest Request) : ICommand<Result>;

public sealed class CreatePatientStatusHistoryCommandHandler : ICommandHandler<CreatePatientStatusHistoryCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreatePatientStatusHistoryCommandHandler> logger;

    public CreatePatientStatusHistoryCommandHandler(
        IPatientWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientStatusHistoryCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreatePatientStatusHistoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "PatientStatusHistories",
                "SequenceNumber",
                cancellationToken,
                ("PatientId", request.PatientId));
            var entity = global::PatientService.Domain.Entities.PatientStatusHistory.Create(request.PatientId, sequence, request.StatusCode, request.From, request.To);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("PatientStatusHistory created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
