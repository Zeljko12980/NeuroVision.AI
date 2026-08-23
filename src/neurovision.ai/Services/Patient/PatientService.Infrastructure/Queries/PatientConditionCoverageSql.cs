namespace PatientService.Infrastructure.Queries;

internal sealed class PatientConditionCoverageSql : IPatientSql<PatientConditionCoverageResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientConditionCoverages" WHERE "PatientId" = @PatientId AND "ConditionCode" = @ConditionCode;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientConditionCoverages" WHERE "PatientId" = @PatientId AND "ConditionCode" = @ConditionCode;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientConditionCoverages" WHERE (@Search IS NULL OR "ConditionCode" ILIKE '%' || @Search || '%' OR COALESCE("Note", '') ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientConditionCoverages"
            WHERE (@Search IS NULL OR "ConditionCode" ILIKE '%' || @Search || '%' OR COALESCE("Note", '') ILIKE '%' || @Search || '%')
            ORDER BY "PatientId", "ConditionCode"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
