namespace PatientService.Infrastructure.Queries;

internal sealed class StatusSql : IPatientSql<PatientStatusResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientStatuses" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientStatuses" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientStatuses" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientStatuses"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
