namespace AppointmentService.Application.Feature.Appointment.Query.GetCatalogs;

public sealed class GetAppointmentCatalogsQueryHandler
    : IQueryHandler<GetAppointmentCatalogsQuery, Result<AppointmentCatalogsResponse>>
{
    private readonly IAppointmentWriteStore writes;

    public GetAppointmentCatalogsQueryHandler(IAppointmentWriteStore writes)
    {
        this.writes = writes;
    }

    public async Task<Result<AppointmentCatalogsResponse>> Handle(
        GetAppointmentCatalogsQuery query,
        CancellationToken cancellationToken)
    {
        var types = await writes.GetTypesAsync(cancellationToken);
        var statuses = await writes.GetStatusesAsync(cancellationToken);

        return Result<AppointmentCatalogsResponse>.Ok(new AppointmentCatalogsResponse
        {
            Types = types.Select(item => item.ToCatalogItem()).ToList(),
            Statuses = statuses.Select(item => item.ToCatalogItem()).ToList()
        });
    }
}
