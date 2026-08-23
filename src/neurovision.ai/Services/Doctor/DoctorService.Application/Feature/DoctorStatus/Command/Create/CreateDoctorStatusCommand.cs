using System.Net;

namespace DoctorService.Application.Feature.DoctorStatus.Command.Create;

public sealed record CreateDoctorStatusCommand(CreateDoctorStatusRequest Request) : ICommand<Result>;

public sealed class CreateDoctorStatusCommandHandler : ICommandHandler<CreateDoctorStatusCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDoctorStatusCommandHandler> logger;

    public CreateDoctorStatusCommandHandler(
        IDoctorWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateDoctorStatusCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDoctorStatusCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::DoctorService.Domain.Entities.DoctorStatus.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::DoctorService.Domain.Entities.DoctorStatus>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("DoctorStatus already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DoctorStatus created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
