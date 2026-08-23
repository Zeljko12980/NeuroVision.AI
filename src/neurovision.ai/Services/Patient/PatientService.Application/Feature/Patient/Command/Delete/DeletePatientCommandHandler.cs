using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace PatientService.Application.Feature.Patient.Command.Delete;

public sealed class DeletePatientCommandHandler
    : ICommandHandler<DeletePatientCommand, Result<bool>>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileStorageService files;
    private readonly IPublishEndpoint publishEndpoint;
    private readonly ILogger<DeletePatientCommandHandler> logger;

    public DeletePatientCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        IFileStorageService files,
        IPublishEndpoint publishEndpoint,
        ILogger<DeletePatientCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.files = files;
        this.publishEndpoint = publishEndpoint;
        this.logger = logger;
    }

    public async Task<Result<bool>> Handle(
        DeletePatientCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Delete patient started. PatientId={PatientId}", command.Id);

        var entity = await writes.FindAsync<global::PatientService.Domain.Entities.Patient>(
            [command.Id],
            cancellationToken);

        if (entity is null)
        {
            logger.LogWarning("Delete patient failed. Patient not found. PatientId={PatientId}", command.Id);
            return Result<bool>.Fail(
                "Patient not found.",
                HttpStatusCode.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(entity.ProfilePictureUrl))
            await files.DeleteFileAsync(entity.ProfilePictureUrl);

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new DeleteUserEvent(command.Id), cancellationToken);

        logger.LogInformation("Patient deleted successfully. PatientId={PatientId}", command.Id);

        return Result<bool>.Ok(true);
    }
}
