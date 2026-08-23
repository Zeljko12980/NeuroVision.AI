namespace PatientService.Infrastructure.Queries;

internal sealed class BloodTypeSql : IPatientSql<BloodTypeResponse>
{
    public string GetByKey => """
        SELECT * FROM "BloodTypes" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "BloodTypes" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "BloodTypes" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "BloodTypes"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
