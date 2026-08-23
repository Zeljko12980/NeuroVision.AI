using System.Net;

namespace DoctorService.Application.Feature.DoctorDegreeCoverage.Command.Create;

public sealed record CreateDoctorDegreeCoverageCommand(CreateDoctorDegreeCoverageRequest Request) : ICommand<Result>;

public sealed class CreateDoctorDegreeCoverageCommandHandler : ICommandHandler<CreateDoctorDegreeCoverageCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDoctorDegreeCoverageCommandHandler> logger;

    public CreateDoctorDegreeCoverageCommandHandler(
        IDoctorWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateDoctorDegreeCoverageCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDoctorDegreeCoverageCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var existing = await writes.FindAsync<global::DoctorService.Domain.Entities.DoctorDegreeCoverage>([request.DoctorId, request.DegreeTypeCode], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("DoctorDegreeCoverage already exists.", HttpStatusCode.Conflict);
            }

            var entity = global::DoctorService.Domain.Entities.DoctorDegreeCoverage.Create(request.DoctorId, request.DegreeTypeCode, request.InstitutionName, request.Year);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DoctorDegreeCoverage created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
