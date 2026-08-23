namespace AppointmentService.Infrastructure.Persistence;

internal sealed class AppointmentWriteStore : IAppointmentWriteStore
{
    private readonly AppointmentDbContext context;

    public AppointmentWriteStore(AppointmentDbContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        await context.Appointments.AddAsync(appointment, cancellationToken);
    }

    public Task<Appointment?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Appointments.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public Task<bool> TypeExistsAsync(string typeCode, CancellationToken cancellationToken = default)
    {
        var code = typeCode.Trim().ToUpperInvariant();
        return context.AppointmentTypes.AnyAsync(item => item.Code == code, cancellationToken);
    }

    public Task<bool> StatusExistsAsync(string statusCode, CancellationToken cancellationToken = default)
    {
        var code = statusCode.Trim().ToUpperInvariant();
        return context.AppointmentStatuses.AnyAsync(item => item.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentType>> GetTypesAsync(CancellationToken cancellationToken = default)
    {
        return await context.AppointmentTypes
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentStatus>> GetStatusesAsync(CancellationToken cancellationToken = default)
    {
        return await context.AppointmentStatuses
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetRangeAsync(
        DateTime from,
        DateTime to,
        Guid? patientId,
        Guid? doctorId,
        CancellationToken cancellationToken = default)
    {
        var query = context.Appointments
            .AsNoTracking()
            .Where(item => item.StartsAt < to && item.EndsAt > from);

        if (patientId.HasValue)
            query = query.Where(item => item.PatientId == patientId.Value);

        if (doctorId.HasValue)
            query = query.Where(item => item.DoctorId == doctorId.Value);

        return await query
            .OrderBy(item => item.StartsAt)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasOverlapAsync(
        Guid doctorId,
        DateTime startsAt,
        DateTime endsAt,
        Guid? excludeId,
        CancellationToken cancellationToken = default)
    {
        var query = context.Appointments.Where(item =>
            item.DoctorId == doctorId
            && item.StatusCode != AppointmentStatusCodes.Cancelled
            && item.StartsAt < endsAt
            && startsAt < item.EndsAt);

        if (excludeId.HasValue)
            query = query.Where(item => item.Id != excludeId.Value);

        return query.AnyAsync(cancellationToken);
    }
}
