using System.Net;

namespace PatientService.Application.Feature.PatientDoctorAssignmentHistory.Command.Create;

public sealed record CreatePatientDoctorAssignmentHistoryCommand(CreatePatientDoctorAssignmentHistoryRequest Request) : ICommand<Result>;

public sealed class CreatePatientDoctorAssignmentHistoryCommandHandler : ICommandHandler<CreatePatientDoctorAssignmentHistoryCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreatePatientDoctorAssignmentHistoryCommandHandler> logger;

    public CreatePatientDoctorAssignmentHistoryCommandHandler(
        IPatientWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreatePatientDoctorAssignmentHistoryCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreatePatientDoctorAssignmentHistoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "PatientDoctorAssignmentHistories",
                "SequenceNumber",
                cancellationToken,
                ("PatientId", request.PatientId));
            var entity = global::PatientService.Domain.Entities.PatientDoctorAssignmentHistory.Create(request.PatientId, sequence, request.DoctorId, request.From, request.To);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("PatientDoctorAssignmentHistory created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
