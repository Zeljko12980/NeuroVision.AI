namespace AppointmentService.Application.Feature.Appointment.Command.Create;

public sealed record CreateAppointmentCommand(CreateAppointmentRequest Request, AppointmentActor Actor)
    : ICommand<Result<AppointmentResponse>>;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.PatientId).NotEmpty();
        RuleFor(x => x.Request.DoctorId).NotEmpty();
        RuleFor(x => x.Request.TypeCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Request.Title).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Request.Notes).MaximumLength(512)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Notes));
        RuleFor(x => x.Request.EndsAt)
            .GreaterThan(x => x.Request.StartsAt)
            .WithMessage("End time must be after start time.");
    }
}
