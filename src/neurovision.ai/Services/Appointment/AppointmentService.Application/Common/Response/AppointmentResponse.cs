namespace AppointmentService.Application.Common.Response;

public class AppointmentResponse
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string TypeCode { get; set; } = null!;
    public string StatusCode { get; set; } = null!;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Title { get; set; } = null!;
    public string? Notes { get; set; }
    public int? HealthInstitutionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CatalogItemResponse
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class AppointmentCatalogsResponse
{
    public IReadOnlyList<CatalogItemResponse> Types { get; set; } = [];
    public IReadOnlyList<CatalogItemResponse> Statuses { get; set; } = [];
}

public class CreateAppointmentRequest
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string TypeCode { get; set; } = null!;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Title { get; set; } = null!;
    public string? Notes { get; set; }
    public int? HealthInstitutionId { get; set; }
}

public class RescheduleAppointmentRequest
{
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Title { get; set; } = null!;
    public string? Notes { get; set; }
}
