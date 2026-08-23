namespace PatientService.Infrastructure.Queries;

internal sealed class PatientConsentCoverageSql : IPatientSql<PatientConsentCoverageResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientConsentCoverages" WHERE "PatientId" = @PatientId AND "ConsentTypeCode" = @ConsentTypeCode;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientConsentCoverages" WHERE "PatientId" = @PatientId AND "ConsentTypeCode" = @ConsentTypeCode;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientConsentCoverages" WHERE (@Search IS NULL OR "ConsentTypeCode" ILIKE '%' || @Search || '%' OR "PatientId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientConsentCoverages"
            WHERE (@Search IS NULL OR "ConsentTypeCode" ILIKE '%' || @Search || '%' OR "PatientId"::text ILIKE '%' || @Search || '%')
            ORDER BY "PatientId", "ConsentTypeCode"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
