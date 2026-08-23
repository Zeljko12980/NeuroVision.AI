namespace DoctorService.Infrastructure.Queries;

internal sealed class DoctorDegreeCoverageSql : IDoctorSql<DoctorDegreeCoverageResponse>
{
    public string GetByKey => """
        SELECT * FROM "DoctorDegreeCoverages" WHERE "DoctorId" = @DoctorId AND "DegreeTypeCode" = @DegreeTypeCode;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "DoctorDegreeCoverages" WHERE "DoctorId" = @DoctorId AND "DegreeTypeCode" = @DegreeTypeCode;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "DoctorDegreeCoverages" WHERE (@Search IS NULL OR "DegreeTypeCode" ILIKE '%' || @Search || '%' OR COALESCE("InstitutionName", '') ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "DoctorDegreeCoverages"
            WHERE (@Search IS NULL OR "DegreeTypeCode" ILIKE '%' || @Search || '%' OR COALESCE("InstitutionName", '') ILIKE '%' || @Search || '%')
            ORDER BY "DoctorId", "DegreeTypeCode"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
