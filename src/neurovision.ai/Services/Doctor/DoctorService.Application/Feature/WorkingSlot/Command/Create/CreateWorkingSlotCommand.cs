using System.Net;

namespace DoctorService.Application.Feature.WorkingSlot.Command.Create;

public sealed record CreateWorkingSlotCommand(CreateWorkingSlotRequest Request) : ICommand<Result>;

public sealed class CreateWorkingSlotCommandHandler : ICommandHandler<CreateWorkingSlotCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateWorkingSlotCommandHandler> logger;

    public CreateWorkingSlotCommandHandler(
        IDoctorWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreateWorkingSlotCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateWorkingSlotCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "WorkingSlots",
                "SequenceNumber",
                cancellationToken,
                ("DoctorId", request.DoctorId),
                ("DayOfWeek", request.DayOfWeek));
            var entity = global::DoctorService.Domain.Entities.WorkingSlot.Create(request.DoctorId, request.DayOfWeek, sequence, request.Start, request.End, request.ValidFrom, request.ValidTo);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("WorkingSlot created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
