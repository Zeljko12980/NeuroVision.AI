namespace PatientService.Application.Feature.InsurancePayer.Command.Create;

public sealed record CreateInsurancePayerCommand(CreateInsurancePayerRequest Request) : ICommand<Result>;

public sealed class CreateInsurancePayerCommandHandler : ICommandHandler<CreateInsurancePayerCommand, Result>
{
    private readonly IPatientWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateInsurancePayerCommandHandler> logger;

    public CreateInsurancePayerCommandHandler(
        IPatientWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateInsurancePayerCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateInsurancePayerCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::PatientService.Domain.Entities.InsurancePayer.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::PatientService.Domain.Entities.InsurancePayer>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("InsurancePayer already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("InsurancePayer created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
