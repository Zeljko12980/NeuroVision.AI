using System.Net;

namespace DoctorService.Application.Feature.LicenseAuthority.Command.Create;

public sealed record CreateLicenseAuthorityCommand(CreateLicenseAuthorityRequest Request) : ICommand<Result>;

public sealed class CreateLicenseAuthorityCommandHandler : ICommandHandler<CreateLicenseAuthorityCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateLicenseAuthorityCommandHandler> logger;

    public CreateLicenseAuthorityCommandHandler(
        IDoctorWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateLicenseAuthorityCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateLicenseAuthorityCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::DoctorService.Domain.Entities.LicenseAuthority.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::DoctorService.Domain.Entities.LicenseAuthority>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("LicenseAuthority already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("LicenseAuthority created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
