namespace AppointmentService.Application.Common.Interfaces;

public interface IAppointmentWriteStore
{
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);

    Task<Appointment?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> TypeExistsAsync(string typeCode, CancellationToken cancellationToken = default);

    Task<bool> StatusExistsAsync(string statusCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppointmentType>> GetTypesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppointmentStatus>> GetStatusesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Appointment>> GetRangeAsync(
        DateTime from,
        DateTime to,
        Guid? patientId,
        Guid? doctorId,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlapAsync(
        Guid doctorId,
        DateTime startsAt,
        DateTime endsAt,
        Guid? excludeId,
        CancellationToken cancellationToken = default);
}
