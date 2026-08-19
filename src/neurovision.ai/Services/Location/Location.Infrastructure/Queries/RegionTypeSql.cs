namespace LocationService.Infrastructure.Queries;

internal sealed class RegionTypeSql : ILocationSql<RegionTypeResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "RegionTypes"
        WHERE "Code" = @Code;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "RegionTypes"
        WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "RegionTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT *
        FROM "RegionTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
