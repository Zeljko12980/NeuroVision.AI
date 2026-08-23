using System.Net;

namespace DoctorService.Application.Feature.DoctorLanguage.Command.Create;

public sealed record CreateDoctorLanguageCommand(CreateDoctorLanguageRequest Request) : ICommand<Result>;

public sealed class CreateDoctorLanguageCommandHandler : ICommandHandler<CreateDoctorLanguageCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDoctorLanguageCommandHandler> logger;

    public CreateDoctorLanguageCommandHandler(
        IDoctorWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateDoctorLanguageCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDoctorLanguageCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::DoctorService.Domain.Entities.Language.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::DoctorService.Domain.Entities.Language>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("DoctorLanguage already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DoctorLanguage created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
