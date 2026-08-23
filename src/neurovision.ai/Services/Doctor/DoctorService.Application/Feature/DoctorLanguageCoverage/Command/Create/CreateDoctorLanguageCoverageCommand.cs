using System.Net;

namespace DoctorService.Application.Feature.DoctorLanguageCoverage.Command.Create;

public sealed record CreateDoctorLanguageCoverageCommand(CreateDoctorLanguageCoverageRequest Request) : ICommand<Result>;

public sealed class CreateDoctorLanguageCoverageCommandHandler : ICommandHandler<CreateDoctorLanguageCoverageCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDoctorLanguageCoverageCommandHandler> logger;

    public CreateDoctorLanguageCoverageCommandHandler(
        IDoctorWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateDoctorLanguageCoverageCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDoctorLanguageCoverageCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var existing = await writes.FindAsync<global::DoctorService.Domain.Entities.DoctorLanguageCoverage>([request.DoctorId, request.LanguageCode], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("DoctorLanguageCoverage already exists.", HttpStatusCode.Conflict);
            }

            var entity = global::DoctorService.Domain.Entities.DoctorLanguageCoverage.Create(request.DoctorId, request.LanguageCode);

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DoctorLanguageCoverage created.");
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
