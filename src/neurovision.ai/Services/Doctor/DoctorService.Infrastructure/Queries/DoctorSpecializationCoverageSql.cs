namespace DoctorService.Infrastructure.Queries;

internal sealed class DoctorSpecializationCoverageSql : IDoctorSql<DoctorSpecializationCoverageResponse>
{
    public string GetByKey => """
        SELECT * FROM "DoctorSpecializationCoverages" WHERE "DoctorId" = @DoctorId AND "SpecializationCode" = @SpecializationCode;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "DoctorSpecializationCoverages" WHERE "DoctorId" = @DoctorId AND "SpecializationCode" = @SpecializationCode;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "DoctorSpecializationCoverages" WHERE (@Search IS NULL OR "SpecializationCode" ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "DoctorSpecializationCoverages"
            WHERE (@Search IS NULL OR "SpecializationCode" ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%')
            ORDER BY "DoctorId", "SpecializationCode"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
