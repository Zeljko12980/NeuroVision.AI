namespace LocationService.Infrastructure.Queries;

internal sealed class GovernmentTypeSql : ILocationSql<GovernmentTypeResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "GovernmentTypes"
        WHERE "Code" = @Code;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "GovernmentTypes"
        WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "GovernmentTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT *
        FROM "GovernmentTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
