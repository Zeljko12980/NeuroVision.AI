namespace PatientService.Infrastructure.Queries;

internal sealed class LanguageSql : IPatientSql<LanguageResponse>
{
    public string GetByKey => """
        SELECT * FROM "Languages" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "Languages" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "Languages" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "Languages"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
