namespace AppointmentService.Application.Common;

public static class AppointmentAccess
{
    public static Result Deny<T>(string message = "You are not allowed to access this appointment.") =>
        Result<T>.Fail(message, HttpStatusCode.Forbidden);

    public static Result Deny(string message = "You are not allowed to access this appointment.") =>
        Result.Fail(message, HttpStatusCode.Forbidden);

    public static bool CanAccess(AppointmentActor actor, Appointment appointment) =>
        actor.IsSuperAdmin
        || appointment.PatientId == actor.UserId
        || appointment.DoctorId == actor.UserId;

    public static Result<(Guid? PatientId, Guid? DoctorId)> ResolveRange(
        AppointmentActor actor,
        Guid? patientId,
        Guid? doctorId)
    {
        if (actor.IsPatient)
            return Result<(Guid?, Guid?)>.Ok((actor.UserId, null));

        if (actor.IsDoctor)
            return Result<(Guid?, Guid?)>.Ok((patientId, actor.UserId));

        if (actor.IsSuperAdmin)
            return Result<(Guid?, Guid?)>.Ok((patientId, doctorId));

        return Result<(Guid?, Guid?)>.Fail(
            "You are not allowed to view appointments.",
            HttpStatusCode.Forbidden);
    }

    public static Result AuthorizeCreate(AppointmentActor actor, CreateAppointmentRequest request)
    {
        if (actor.IsSuperAdmin)
            return Result.Ok();

        if (actor.IsPatient && request.PatientId == actor.UserId)
            return Result.Ok();

        if (actor.IsDoctor && request.DoctorId == actor.UserId)
            return Result.Ok();

        return Deny("You are not allowed to create this appointment.");
    }

    public static Result AuthorizeMutate(AppointmentActor actor, Appointment appointment)
    {
        if (CanAccess(actor, appointment))
            return Result.Ok();

        return Deny();
    }
}
