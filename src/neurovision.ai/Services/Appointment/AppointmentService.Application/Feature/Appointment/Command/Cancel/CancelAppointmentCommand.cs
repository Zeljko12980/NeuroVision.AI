namespace AppointmentService.Application.Feature.Appointment.Command.Cancel;

public sealed record CancelAppointmentCommand(Guid Id, AppointmentActor Actor)
    : ICommand<Result<AppointmentResponse>>;

public sealed class CancelAppointmentCommandValidator : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
