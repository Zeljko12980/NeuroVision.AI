using System.Net;

namespace DoctorService.Application.Feature.DoctorAffiliationHistory.Command.Create;

public sealed record CreateDoctorAffiliationHistoryCommand(CreateDoctorAffiliationHistoryRequest Request) : ICommand<Result>;

public sealed class CreateDoctorAffiliationHistoryCommandHandler : ICommandHandler<CreateDoctorAffiliationHistoryCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDoctorAffiliationHistoryCommandHandler> logger;

    public CreateDoctorAffiliationHistoryCommandHandler(
        IDoctorWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreateDoctorAffiliationHistoryCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDoctorAffiliationHistoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "DoctorAffiliationHistories",
                "SequenceNumber",
                cancellationToken,
                ("DoctorId", request.DoctorId));
            var entity = global::DoctorService.Domain.Entities.DoctorAffiliationHistory.Create(request.DoctorId, sequence, request.InstitutionName, request.HealthInstitutionId, request.From, request.To);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DoctorAffiliationHistory created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
