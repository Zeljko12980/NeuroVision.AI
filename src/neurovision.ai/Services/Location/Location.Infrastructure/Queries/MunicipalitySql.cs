namespace LocationService.Infrastructure.Queries;

internal sealed class MunicipalitySql : ILocationSql<MunicipalityResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "Municipalities"
        WHERE "CountryCode" = @CountryCode AND "Code" = @Code;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "Municipalities"
        WHERE "CountryCode" = @CountryCode AND "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "Municipalities"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT *
        FROM "Municipalities"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
