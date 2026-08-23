namespace PatientService.Application.Feature.PatientEmergencyContact.Command.Create;

public sealed record CreatePatientEmergencyContactCommand(CreatePatientEmergencyContactRequest Request) : ICommand<Result>;

public sealed class CreatePatientEmergencyContactCommandHandler : ICommandHandler<CreatePatientEmergencyContactCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreatePatientEmergencyContactCommandHandler> logger;

    public CreatePatientEmergencyContactCommandHandler(
        IPatientWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientEmergencyContactCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreatePatientEmergencyContactCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "PatientEmergencyContacts",
                "SequenceNumber",
                cancellationToken,
                ("PatientId", request.PatientId));
            var entity = global::PatientService.Domain.Entities.PatientEmergencyContact.Create(request.PatientId, sequence, request.FullName, request.Phone, request.RelationshipCode);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("PatientEmergencyContact created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
