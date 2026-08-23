using System.Net;

namespace DoctorService.Application.Feature.DoctorStatusHistory.Command.Create;

public sealed record CreateDoctorStatusHistoryCommand(CreateDoctorStatusHistoryRequest Request) : ICommand<Result>;

public sealed class CreateDoctorStatusHistoryCommandHandler : ICommandHandler<CreateDoctorStatusHistoryCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly ISequenceStore sequences;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDoctorStatusHistoryCommandHandler> logger;

    public CreateDoctorStatusHistoryCommandHandler(
        IDoctorWriteStore writes,
        ISequenceStore sequences,
        IUnitOfWork unitOfWork,
        ILogger<CreateDoctorStatusHistoryCommandHandler> logger)
    {
        this.writes = writes;
        this.sequences = sequences;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDoctorStatusHistoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var sequence = await sequences.NextAsync(
                "DoctorStatusHistories",
                "SequenceNumber",
                cancellationToken,
                ("DoctorId", request.DoctorId));
            var entity = global::DoctorService.Domain.Entities.DoctorStatusHistory.Create(request.DoctorId, sequence, request.StatusCode, request.From, request.To);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DoctorStatusHistory created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
