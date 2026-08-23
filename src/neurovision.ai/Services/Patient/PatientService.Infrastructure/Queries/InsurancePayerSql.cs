namespace PatientService.Infrastructure.Queries;

internal sealed class InsurancePayerSql : IPatientSql<InsurancePayerResponse>
{
    public string GetByKey => """
        SELECT * FROM "InsurancePayers" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "InsurancePayers" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "InsurancePayers" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "InsurancePayers"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
