using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace PatientService.Application.Feature.Patient.Command.Create;

public sealed class CreatePatientCommandHandler
    : ICommandHandler<CreatePatientCommand, Result<PatientResponse>>
{
    private readonly IPatientWriteStore writes;
    private readonly IPatientReadStore<GenderResponse> genders;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileStorageService files;
    private readonly IPublishEndpoint publishEndpoint;
    private readonly ILogger<CreatePatientCommandHandler> logger;

    public CreatePatientCommandHandler(
        IPatientWriteStore writes,
        IPatientReadStore<GenderResponse> genders,
        IUnitOfWork unitOfWork,
        IFileStorageService files,
        IPublishEndpoint publishEndpoint,
        ILogger<CreatePatientCommandHandler> logger)
    {
        this.writes = writes;
        this.genders = genders;
        this.unitOfWork = unitOfWork;
        this.files = files;
        this.publishEndpoint = publishEndpoint;
        this.logger = logger;
    }

    public async Task<Result<PatientResponse>> Handle(
        CreatePatientCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        logger.LogInformation("Create patient started. Email={Email}", request.Email);

        var gender = await ResolveGenderAsync(request.Gender, cancellationToken);
        if (gender is null)
        {
            logger.LogWarning(
                "Create patient failed. Gender not found. Email={Email}, Gender={Gender}",
                request.Email,
                request.Gender);
            return Result<PatientResponse>.Fail(
                $"Gender '{request.Gender}' was not found.",
                HttpStatusCode.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(request.BloodType))
        {
            var bloodType = await writes.FindAsync<BloodType>(
                [request.BloodType.Trim().ToUpperInvariant()],
                cancellationToken);

            if (bloodType is null)
            {
                logger.LogWarning(
                    "Create patient failed. Blood type not found. Email={Email}, BloodType={BloodType}",
                    request.Email,
                    request.BloodType);
                return Result<PatientResponse>.Fail(
                    $"Blood type '{request.BloodType}' was not found.",
                    HttpStatusCode.NotFound);
            }
        }

        var pictureUrl = request.Picture is { Length: > 0 }
            ? await files.SaveFileAsync(request.Picture, "patients")
            : null;

        var statusCode = request.AutoActivate
            ? PatientStatusCodes.Active
            : PatientStatusCodes.PendingVerification;

        var patient = global::PatientService.Domain.Entities.Patient.Create(
            Guid.NewGuid(),
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.DateOfBirth,
            gender.Code,
            statusCode,
            EmptyToNull(request.BloodType),
            request.NationalId,
            pictureUrl,
            request.Notes,
            request.HealthInstitutionId,
            request.Hospital,
            request.AssignedDoctorId,
            request.AddressLine,
            request.SettlementId,
            request.MunicipalityId,
            request.CountryId,
            request.HeightCm,
            request.WeightKg,
            DateTime.UtcNow);

        foreach (var code in ParseCodes(request.Languages))
        {
            var language = await writes.FindAsync<Language>([code], cancellationToken);
            if (language is not null)
                patient.AddLanguage(code);
        }

        foreach (var code in ParseCodes(request.Allergies))
        {
            var allergy = await writes.FindAsync<Allergy>([code], cancellationToken);
            if (allergy is not null)
                patient.AddAllergy(code);
        }

        foreach (var code in ParseCodes(request.Conditions))
        {
            var condition = await writes.FindAsync<Condition>([code], cancellationToken);
            if (condition is not null)
                patient.AddCondition(code);
        }

        if (!string.IsNullOrWhiteSpace(request.InsurancePayerCode)
            && !string.IsNullOrWhiteSpace(request.InsurancePolicyNumber))
        {
            var payer = await writes.FindAsync<InsurancePayer>(
                [request.InsurancePayerCode.Trim().ToUpperInvariant()],
                cancellationToken);

            if (payer is not null)
                patient.ChangeInsurance(payer.Code, request.InsurancePolicyNumber, DateTime.UtcNow);
        }

        if (!string.IsNullOrWhiteSpace(request.EmergencyContactName)
            && !string.IsNullOrWhiteSpace(request.EmergencyContactPhone)
            && !string.IsNullOrWhiteSpace(request.EmergencyRelationshipCode))
        {
            patient.AddEmergencyContact(
                request.EmergencyContactName,
                request.EmergencyContactPhone,
                request.EmergencyRelationshipCode);
        }

        patient.GrantConsent("DATA", DateTime.UtcNow);
        patient.GrantConsent("IMG", DateTime.UtcNow);

        await writes.AddAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new CreateUserEvent(
                patient.Id,
                $"{request.FirstName}.{request.LastName}".ToLowerInvariant(),
                request.Email,
                "Patient"),
            cancellationToken);

        logger.LogInformation(
            "Patient created successfully. PatientId={PatientId}, Email={Email}",
            patient.Id,
            patient.Email);

        return Result<PatientResponse>.Created(patient.ToResponse());
    }

    private async Task<GenderResponse?> ResolveGenderAsync(
        string value,
        CancellationToken cancellationToken)
    {
        var trimmed = value.Trim();
        return await genders.GetByKeyAsync(
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
            if (code.Length is >= 1 and <= 10)
                yield return code;
        }
    }

    private static string? EmptyToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
