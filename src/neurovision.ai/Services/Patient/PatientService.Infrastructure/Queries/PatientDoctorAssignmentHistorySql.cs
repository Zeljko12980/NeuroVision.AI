namespace PatientService.Infrastructure.Queries;

internal sealed class PatientDoctorAssignmentHistorySql : IPatientSql<PatientDoctorAssignmentHistoryResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientDoctorAssignmentHistories" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientDoctorAssignmentHistories" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientDoctorAssignmentHistories" WHERE (@Search IS NULL OR "PatientId"::text ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientDoctorAssignmentHistories"
            WHERE (@Search IS NULL OR "PatientId"::text ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%')
            ORDER BY "PatientId", "SequenceNumber" DESC
            LIMIT @PageSize OFFSET @Offset;
        """;
}
