namespace AppointmentService.Application.Feature.Appointment.Query.GetCatalogs;

public sealed record GetAppointmentCatalogsQuery()
    : IQuery<Result<AppointmentCatalogsResponse>>;
