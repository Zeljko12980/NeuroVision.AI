namespace PatientService.Infrastructure.Queries;

internal sealed class AllergySql : IPatientSql<AllergyResponse>
{
    public string GetByKey => """
        SELECT * FROM "Allergies" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "Allergies" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "Allergies" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "Allergies"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
