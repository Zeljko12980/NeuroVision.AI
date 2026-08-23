using BuildingBlocks.Auth;

namespace AppointmentService.Application.Common;

public sealed record AppointmentActor(Guid UserId, string Role)
{
    public bool IsSuperAdmin =>
        Role.Equals(AuthRoles.SuperAdministrator, StringComparison.OrdinalIgnoreCase);

    public bool IsDoctor =>
        Role.Equals(AuthRoles.Doctor, StringComparison.OrdinalIgnoreCase);

    public bool IsPatient =>
        Role.Equals(AuthRoles.Patient, StringComparison.OrdinalIgnoreCase);

    public bool IsStaff => IsSuperAdmin || IsDoctor;
}
