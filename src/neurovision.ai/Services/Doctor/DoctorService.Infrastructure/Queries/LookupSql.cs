namespace DoctorService.Infrastructure.Queries;

internal sealed class LookupSql<TResponse> : IDoctorSql<TResponse>
{
    private readonly string table;

    public LookupSql(string table)
    {
        this.table = table;
    }

    public string GetByKey => $"""
        SELECT *
        FROM "{table}"
        WHERE "Code" = @Code
           OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => $"""
        SELECT COUNT(*)
        FROM "{table}"
        WHERE "Code" = @Code;
        """;

    public string Count => $"""
        SELECT COUNT(*)
        FROM "{table}"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => $"""
        SELECT *
        FROM "{table}"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
