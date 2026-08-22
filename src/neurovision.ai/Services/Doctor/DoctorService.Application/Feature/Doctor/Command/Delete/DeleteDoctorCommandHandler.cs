using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace DoctorService.Application.Feature.Doctor.Command.Delete;

public sealed class DeleteDoctorCommandHandler
    : ICommandHandler<DeleteDoctorCommand, Result<bool>>
{
    private readonly IDoctorWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileStorageService files;
    private readonly IPublishEndpoint publishEndpoint;
    private readonly ILogger<DeleteDoctorCommandHandler> logger;

    public DeleteDoctorCommandHandler(
        IDoctorWriteStore writes,
        IUnitOfWork unitOfWork,
        IFileStorageService files,
        IPublishEndpoint publishEndpoint,
        ILogger<DeleteDoctorCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.files = files;
        this.publishEndpoint = publishEndpoint;
        this.logger = logger;
    }

    public async Task<Result<bool>> Handle(
        DeleteDoctorCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Delete doctor started. DoctorId={DoctorId}", command.Id);

        var entity = await writes.FindAsync<global::DoctorService.Domain.Entities.Doctor>(
            new object[] { command.Id },
            cancellationToken);

        if (entity is null)
        {
            logger.LogWarning("Delete doctor failed. Doctor not found. DoctorId={DoctorId}", command.Id);
            return Result<bool>.Fail(
                "Doctor not found.",
                HttpStatusCode.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(entity.ProfilePictureUrl))
            await files.DeleteFileAsync(entity.ProfilePictureUrl);

        writes.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new DeleteUserEvent(command.Id), cancellationToken);

        logger.LogInformation("Doctor deleted successfully. DoctorId={DoctorId}", command.Id);

        return Result<bool>.Ok(true);
    }
}
