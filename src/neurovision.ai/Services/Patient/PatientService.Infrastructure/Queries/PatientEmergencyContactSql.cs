namespace PatientService.Infrastructure.Queries;

internal sealed class PatientEmergencyContactSql : IPatientSql<PatientEmergencyContactResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientEmergencyContacts" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientEmergencyContacts" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientEmergencyContacts" WHERE (@Search IS NULL OR "FullName" ILIKE '%' || @Search || '%' OR "Phone" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientEmergencyContacts"
            WHERE (@Search IS NULL OR "FullName" ILIKE '%' || @Search || '%' OR "Phone" ILIKE '%' || @Search || '%')
            ORDER BY "PatientId", "SequenceNumber"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
