namespace AppointmentService.Application.Feature.Appointment.Query.GetById;

public sealed record GetAppointmentByIdQuery(Guid Id, AppointmentActor Actor)
    : IQuery<Result<AppointmentResponse>>;
