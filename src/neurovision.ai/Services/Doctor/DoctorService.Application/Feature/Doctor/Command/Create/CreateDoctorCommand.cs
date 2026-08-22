namespace DoctorService.Application.Feature.Doctor.Command.Create;

public sealed record CreateDoctorCommand(CreateDoctorRequest Request) : ICommand<Result<DoctorResponse>>;

public sealed class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorCommandValidator()
    {
        RuleFor(x => x.Request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.LicenseNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Request.LicenseAuthorityCode)
            .MaximumLength(10)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.LicenseAuthorityCode));
        RuleFor(x => x.Request.Specialization).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[0-9]{7,15}$");
        RuleFor(x => x.Request.Languages).NotEmpty();
    }
}
