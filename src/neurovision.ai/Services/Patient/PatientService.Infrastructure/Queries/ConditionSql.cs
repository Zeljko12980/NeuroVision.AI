namespace PatientService.Infrastructure.Queries;

internal sealed class ConditionSql : IPatientSql<ConditionResponse>
{
    public string GetByKey => """
        SELECT * FROM "Conditions" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "Conditions" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "Conditions" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "Conditions"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
