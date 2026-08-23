namespace AppointmentService.Application.Common.Mappings;

public static class AppointmentMappings
{
    public static AppointmentResponse ToResponse(this Appointment entity) =>
        new()
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            TypeCode = entity.TypeCode,
            StatusCode = entity.StatusCode,
            StartsAt = entity.StartsAt,
            EndsAt = entity.EndsAt,
            Title = entity.Title,
            Notes = entity.Notes,
            HealthInstitutionId = entity.HealthInstitutionId,
            CreatedAt = entity.CreatedAt,
            CancelledAt = entity.CancelledAt,
            CompletedAt = entity.CompletedAt
        };

    public static CatalogItemResponse ToCatalogItem(this AppointmentType entity) =>
        new()
        {
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description
        };

    public static CatalogItemResponse ToCatalogItem(this AppointmentStatus entity) =>
        new()
        {
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description
        };
}
