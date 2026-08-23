namespace PatientService.Infrastructure.Queries;

internal sealed class PatientLanguageCoverageSql : IPatientSql<PatientLanguageCoverageResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientLanguageCoverages" WHERE "PatientId" = @PatientId AND "LanguageCode" = @LanguageCode;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientLanguageCoverages" WHERE "PatientId" = @PatientId AND "LanguageCode" = @LanguageCode;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientLanguageCoverages" WHERE (@Search IS NULL OR "LanguageCode" ILIKE '%' || @Search || '%' OR "PatientId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientLanguageCoverages"
            WHERE (@Search IS NULL OR "LanguageCode" ILIKE '%' || @Search || '%' OR "PatientId"::text ILIKE '%' || @Search || '%')
            ORDER BY "PatientId", "LanguageCode"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
