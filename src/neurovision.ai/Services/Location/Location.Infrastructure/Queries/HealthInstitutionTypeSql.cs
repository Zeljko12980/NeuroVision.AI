namespace LocationService.Infrastructure.Queries;

internal sealed class HealthInstitutionTypeSql : ILocationSql<HealthInstitutionTypeResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "HealthInstitutionTypes"
        WHERE "Code" = @Code;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "HealthInstitutionTypes"
        WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "HealthInstitutionTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT *
        FROM "HealthInstitutionTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
