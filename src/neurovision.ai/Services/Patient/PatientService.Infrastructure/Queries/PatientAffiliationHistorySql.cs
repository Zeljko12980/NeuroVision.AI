namespace PatientService.Infrastructure.Queries;

internal sealed class PatientAffiliationHistorySql : IPatientSql<PatientAffiliationHistoryResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientAffiliationHistories" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientAffiliationHistories" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientAffiliationHistories" WHERE (@Search IS NULL OR "InstitutionName" ILIKE '%' || @Search || '%' OR "PatientId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientAffiliationHistories"
            WHERE (@Search IS NULL OR "InstitutionName" ILIKE '%' || @Search || '%' OR "PatientId"::text ILIKE '%' || @Search || '%')
            ORDER BY "PatientId", "SequenceNumber" DESC
            LIMIT @PageSize OFFSET @Offset;
        """;
}
