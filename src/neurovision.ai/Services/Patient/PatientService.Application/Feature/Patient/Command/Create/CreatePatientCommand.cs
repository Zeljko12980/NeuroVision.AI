namespace PatientService.Application.Feature.Patient.Command.Create;

public sealed record CreatePatientCommand(CreatePatientRequest Request)
    : ICommand<Result<PatientResponse>>;

public sealed class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.Request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[0-9]{7,15}$");
        RuleFor(x => x.Request.DateOfBirth)
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .GreaterThan(new DateTime(1900, 1, 1));
        RuleFor(x => x.Request.Gender).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Request.Languages).NotEmpty();
        RuleFor(x => x.Request.NationalId).MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.NationalId));
        RuleFor(x => x.Request.HeightCm)
            .InclusiveBetween(1, 300)
            .When(x => x.Request.HeightCm.HasValue);
        RuleFor(x => x.Request.WeightKg)
            .InclusiveBetween(1, 500)
            .When(x => x.Request.WeightKg.HasValue);
    }
}
