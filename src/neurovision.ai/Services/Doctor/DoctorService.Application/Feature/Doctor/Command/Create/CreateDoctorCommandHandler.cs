using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace DoctorService.Application.Feature.Doctor.Command.Create;

public sealed class CreateDoctorCommandHandler
    : ICommandHandler<CreateDoctorCommand, Result<DoctorResponse>>
{
    private readonly IDoctorWriteStore writes;
    private readonly IDoctorReadStore<SpecializationResponse> specializations;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileStorageService files;
    private readonly IPublishEndpoint publishEndpoint;
    private readonly ILogger<CreateDoctorCommandHandler> logger;

    public CreateDoctorCommandHandler(
        IDoctorWriteStore writes,
        IDoctorReadStore<SpecializationResponse> specializations,
        IUnitOfWork unitOfWork,
        IFileStorageService files,
        IPublishEndpoint publishEndpoint,
        ILogger<CreateDoctorCommandHandler> logger)
    {
        this.writes = writes;
        this.specializations = specializations;
        this.unitOfWork = unitOfWork;
        this.files = files;
        this.publishEndpoint = publishEndpoint;
        this.logger = logger;
    }

    public async Task<Result<DoctorResponse>> Handle(
        CreateDoctorCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        logger.LogInformation("Create doctor started. Email={Email}", request.Email);

        var specialization = await ResolveSpecializationAsync(request.Specialization, cancellationToken);
        if (specialization is null)
        {
            logger.LogWarning(
                "Create doctor failed. Specialization not found. Email={Email}, Specialization={Specialization}",
                request.Email,
                request.Specialization);
            return Result<DoctorResponse>.Fail(
                $"Specialization '{request.Specialization}' was not found.",
                HttpStatusCode.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(request.LicenseAuthorityCode))
        {
            var authority = await writes.FindAsync<LicenseAuthority>(
                new object[] { request.LicenseAuthorityCode.Trim().ToUpperInvariant() },
                cancellationToken);

            if (authority is null)
            {
                logger.LogWarning(
                    "Create doctor failed. License authority not found. Email={Email}, LicenseAuthority={LicenseAuthority}",
                    request.Email,
                    request.LicenseAuthorityCode);
                return Result<DoctorResponse>.Fail(
                    $"License authority '{request.LicenseAuthorityCode}' was not found.",
                    HttpStatusCode.NotFound);
            }
        }

        var pictureUrl = request.Picture is { Length: > 0 }
            ? await files.SaveFileAsync(request.Picture, "doctors")
            : null;

        var statusCode = request.AutoActivate
            ? DoctorStatusCodes.Active
            : DoctorStatusCodes.PendingVerification;

        var doctor = global::DoctorService.Domain.Entities.Doctor.Create(
            Guid.NewGuid(),
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.LicenseNumber,
            EmptyToNull(request.LicenseAuthorityCode),
            specialization.Code,
            statusCode,
            pictureUrl,
            request.Bio,
            request.HealthInstitutionId,
            request.Hospital,
            request.IsAvailable,
            DateTime.UtcNow);

        foreach (var code in ParseCodes(request.Languages))
        {
            var language = await writes.FindAsync<Language>(new object[] { code }, cancellationToken);
            if (language is not null)
                doctor.AddLanguage(code);
        }

        foreach (var code in ParseCodes(request.Degrees))
        {
            var degree = await writes.FindAsync<DegreeType>(new object[] { code }, cancellationToken);
            if (degree is not null)
                doctor.AddDegree(code);
        }

        await writes.AddAsync(doctor, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new CreateUserEvent(
                doctor.Id,
                $"{request.FirstName}.{request.LastName}".ToLowerInvariant(),
                request.Email,
                "Doctor"),
            cancellationToken);

        logger.LogInformation(
            "Doctor created successfully. DoctorId={DoctorId}, Email={Email}",
            doctor.Id,
            doctor.Email);

        return Result<DoctorResponse>.Created(doctor.ToResponse());
    }

    private async Task<SpecializationResponse?> ResolveSpecializationAsync(
        string value,
        CancellationToken cancellationToken)
    {
        var trimmed = value.Trim();
        return await specializations.GetByKeyAsync(
            new { Code = trimmed.ToUpperInvariant(), Name = trimmed },
            cancellationToken);
    }

    private static IEnumerable<string> ParseCodes(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            yield break;

        foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var code = part.ToUpperInvariant();
            if (code.Length is >= 2 and <= 10)
                yield return code;
        }
    }

    private static string? EmptyToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
