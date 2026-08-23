using System.Net;

namespace DoctorService.Application.Feature.DegreeType.Command.Create;

public sealed record CreateDegreeTypeCommand(CreateDegreeTypeRequest Request) : ICommand<Result>;

public sealed class CreateDegreeTypeCommandHandler : ICommandHandler<CreateDegreeTypeCommand, Result>
{
    private readonly IDoctorWriteStore writes;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CreateDegreeTypeCommandHandler> logger;

    public CreateDegreeTypeCommandHandler(
        IDoctorWriteStore writes,
        IUnitOfWork unitOfWork,
        ILogger<CreateDegreeTypeCommandHandler> logger)
    {
        this.writes = writes;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateDegreeTypeCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        try
        {
            var entity = global::DoctorService.Domain.Entities.DegreeType.Create(request.Code, request.Name, request.Description);
            var existing = await writes.FindAsync<global::DoctorService.Domain.Entities.DegreeType>([entity.Code], cancellationToken);
            if (existing is not null)
            {
                return Result.Fail("DegreeType already exists.", HttpStatusCode.Conflict);
            }

            await writes.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("DegreeType created. Code={Code}", entity.Code);
            return Result.Created();
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message, HttpStatusCode.BadRequest);
        }
    }
}
