namespace AppointmentService.Application.Feature.Appointment.Query.GetRange;

public sealed record GetAppointmentRangeQuery(
    DateTime From,
    DateTime To,
    AppointmentActor Actor,
    Guid? PatientId,
    Guid? DoctorId) : IQuery<Result<IReadOnlyList<AppointmentResponse>>>;
