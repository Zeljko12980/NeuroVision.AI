using System.Net;

namespace DoctorService.Application.Feature.Specialization.Command.Create;

public sealed record CreateSpecializationCommand(CreateSpecializationRequest Request) : ICommand<Result>;

public sealed class CreateSpecializationCommandHandler : ICommandHandler<CreateSpecializationCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateSpecializationCommandHandler> logger;

    public CreateSpecializationCommandHandler(
        IDoctorWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateSpecializationCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateSpecializationCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::DoctorService.Domain.Entities.Specialization.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::DoctorService.Domain.Entities.Specialization>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("Specialization already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Specialization created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
