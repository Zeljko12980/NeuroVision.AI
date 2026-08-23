namespace DoctorService.Infrastructure.Queries;

internal sealed class DegreeTypeSql : IDoctorSql<DegreeTypeResponse>
{
    public string GetByKey => """
        SELECT * FROM "DegreeTypes" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "DegreeTypes" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "DegreeTypes" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "DegreeTypes"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
