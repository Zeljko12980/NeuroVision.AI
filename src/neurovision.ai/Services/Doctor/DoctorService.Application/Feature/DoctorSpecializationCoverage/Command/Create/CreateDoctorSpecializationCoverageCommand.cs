using System.Net;

namespace DoctorService.Application.Feature.DoctorSpecializationCoverage.Command.Create;

public sealed record CreateDoctorSpecializationCoverageCommand(CreateDoctorSpecializationCoverageRequest Request) : ICommand<Result>;

public sealed class CreateDoctorSpecializationCoverageCommandHandler : ICommandHandler<CreateDoctorSpecializationCoverageCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDoctorSpecializationCoverageCommandHandler> logger;

    public CreateDoctorSpecializationCoverageCommandHandler(
        IDoctorWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateDoctorSpecializationCoverageCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDoctorSpecializationCoverageCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var existing = await writes.FindAsync<global::DoctorService.Domain.Entities.DoctorSpecializationCoverage>([request.DoctorId, request.SpecializationCode], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("DoctorSpecializationCoverage already exists.", HttpStatusCode.Conflict);
            }

            var entity = global::DoctorService.Domain.Entities.DoctorSpecializationCoverage.Create(request.DoctorId, request.SpecializationCode, request.IsPrimary, request.From, request.To);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DoctorSpecializationCoverage created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
