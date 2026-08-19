namespace LocationService.Infrastructure.Queries;

internal sealed class HealthInstitutionSql : ILocationSql<HealthInstitutionResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "HealthInstitutions"
        WHERE "Id" = @Id;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "HealthInstitutions"
        WHERE "Id" = @Id;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "HealthInstitutions"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT *
        FROM "HealthInstitutions"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
