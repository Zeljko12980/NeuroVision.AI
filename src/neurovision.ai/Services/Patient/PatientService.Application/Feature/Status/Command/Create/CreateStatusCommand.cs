namespace PatientService.Application.Feature.Status.Command.Create;

public sealed record CreateStatusCommand(CreateStatusRequest Request) : ICommand<Result>;

public sealed class CreateStatusCommandHandler : ICommandHandler<CreateStatusCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateStatusCommandHandler> logger;

    public CreateStatusCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateStatusCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateStatusCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::PatientService.Domain.Entities.PatientStatus.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.PatientStatus>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("Status already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Status created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
