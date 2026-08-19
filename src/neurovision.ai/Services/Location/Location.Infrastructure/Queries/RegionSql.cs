namespace LocationService.Infrastructure.Queries;

internal sealed class RegionSql : ILocationSql<RegionResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "Regions"
        WHERE "TypeCode" = @TypeCode AND "Code" = @Code;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "Regions"
        WHERE "TypeCode" = @TypeCode AND "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "Regions"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT *
        FROM "Regions"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
