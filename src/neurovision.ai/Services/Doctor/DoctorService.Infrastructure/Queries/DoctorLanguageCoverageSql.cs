namespace DoctorService.Infrastructure.Queries;

internal sealed class DoctorLanguageCoverageSql : IDoctorSql<DoctorLanguageCoverageResponse>
{
    public string GetByKey => """
        SELECT * FROM "DoctorLanguageCoverages" WHERE "DoctorId" = @DoctorId AND "LanguageCode" = @LanguageCode;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "DoctorLanguageCoverages" WHERE "DoctorId" = @DoctorId AND "LanguageCode" = @LanguageCode;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "DoctorLanguageCoverages" WHERE (@Search IS NULL OR "LanguageCode" ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "DoctorLanguageCoverages"
            WHERE (@Search IS NULL OR "LanguageCode" ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%')
            ORDER BY "DoctorId", "LanguageCode"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
