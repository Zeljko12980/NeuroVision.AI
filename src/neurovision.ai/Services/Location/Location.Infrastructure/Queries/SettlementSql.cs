namespace LocationService.Infrastructure.Queries;

internal sealed class SettlementSql : ILocationSql<SettlementResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "Settlements"
        WHERE "CountryCode" = @CountryCode AND "Code" = @Code;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "Settlements"
        WHERE "CountryCode" = @CountryCode AND "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "Settlements"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT *
        FROM "Settlements"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
