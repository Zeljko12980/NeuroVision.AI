namespace PatientService.Application.Feature.BloodType.Command.Create;

public sealed record CreateBloodTypeCommand(CreateBloodTypeRequest Request) : ICommand<Result>;

public sealed class CreateBloodTypeCommandHandler : ICommandHandler<CreateBloodTypeCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateBloodTypeCommandHandler> logger;

    public CreateBloodTypeCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateBloodTypeCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateBloodTypeCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::PatientService.Domain.Entities.BloodType.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.BloodType>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("BloodType already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("BloodType created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
