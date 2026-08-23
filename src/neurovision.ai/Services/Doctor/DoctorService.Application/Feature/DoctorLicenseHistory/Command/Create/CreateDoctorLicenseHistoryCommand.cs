using System.Net;

namespace DoctorService.Application.Feature.DoctorLicenseHistory.Command.Create;

public sealed record CreateDoctorLicenseHistoryCommand(CreateDoctorLicenseHistoryRequest Request) : ICommand<Result>;

public sealed class CreateDoctorLicenseHistoryCommandHandler : ICommandHandler<CreateDoctorLicenseHistoryCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDoctorLicenseHistoryCommandHandler> logger;

    public CreateDoctorLicenseHistoryCommandHandler(
        IDoctorWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreateDoctorLicenseHistoryCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDoctorLicenseHistoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "DoctorLicenseHistories",
                "SequenceNumber",
                cancellationToken,
                ("DoctorId", request.DoctorId));
            var entity = global::DoctorService.Domain.Entities.DoctorLicenseHistory.Create(request.DoctorId, sequence, request.LicenseNumber, request.LicenseAuthorityCode, request.From, request.To);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DoctorLicenseHistory created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
