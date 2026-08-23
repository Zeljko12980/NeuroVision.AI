namespace PatientService.Infrastructure.Queries;

internal sealed class ConsentTypeSql : IPatientSql<ConsentTypeResponse>
{
    public string GetByKey => """
        SELECT * FROM "ConsentTypes" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "ConsentTypes" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "ConsentTypes" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "ConsentTypes"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
