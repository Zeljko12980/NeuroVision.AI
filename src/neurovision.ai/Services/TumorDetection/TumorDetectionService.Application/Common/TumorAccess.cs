using BuildingBlocks.Exceptions;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Application.Common;

public static class TumorAccess
{
    public static Guid? ResolvePatientFilter(TumorActor actor, Guid? patientId) =>
        actor.IsPatient ? actor.UserId : patientId;

    public static void EnsureCanAccessScan(TumorActor actor, BrainScan scan)
    {
        if (actor.IsStaff || scan.PatientId == actor.UserId)
            return;

        throw new NotFoundException($"Brain scan {scan.Id} not found.");
    }

    public static void EnsureCanAccessAnalysis(TumorActor actor, TumorAnalysis analysis)
    {
        if (actor.IsStaff || analysis.BrainScan.PatientId == actor.UserId)
            return;

        throw new NotFoundException($"Analysis {analysis.Id} not found.");
    }

    public static void EnsureCanUploadFor(TumorActor actor, Guid patientId)
    {
        if (actor.IsStaff || patientId == actor.UserId)
            return;

        throw new UnauthorizedAccessException("You are not allowed to upload a scan for this patient.");
    }

    public static void EnsureStaff(TumorActor actor)
    {
        if (actor.IsStaff)
            return;

        throw new UnauthorizedAccessException("Only doctors can perform this action.");
    }
}
