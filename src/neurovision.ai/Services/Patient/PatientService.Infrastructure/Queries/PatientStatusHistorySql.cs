namespace PatientService.Infrastructure.Queries;

internal sealed class PatientStatusHistorySql : IPatientSql<PatientStatusHistoryResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientStatusHistories" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientStatusHistories" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientStatusHistories" WHERE (@Search IS NULL OR "StatusCode" ILIKE '%' || @Search || '%' OR "PatientId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientStatusHistories"
            WHERE (@Search IS NULL OR "StatusCode" ILIKE '%' || @Search || '%' OR "PatientId"::text ILIKE '%' || @Search || '%')
            ORDER BY "PatientId", "SequenceNumber" DESC
            LIMIT @PageSize OFFSET @Offset;
        """;
}
