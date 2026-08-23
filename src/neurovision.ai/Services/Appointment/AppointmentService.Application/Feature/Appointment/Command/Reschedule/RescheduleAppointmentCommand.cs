namespace AppointmentService.Application.Feature.Appointment.Command.Reschedule;

public sealed record RescheduleAppointmentCommand(
    Guid Id,
    RescheduleAppointmentRequest Request,
    AppointmentActor Actor) : ICommand<Result<AppointmentResponse>>;

public sealed class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.Title).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Request.Notes).MaximumLength(512)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Notes));
        RuleFor(x => x.Request.EndsAt)
            .GreaterThan(x => x.Request.StartsAt)
            .WithMessage("End time must be after start time.");
    }
}
