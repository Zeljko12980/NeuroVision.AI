namespace DoctorService.Infrastructure.Queries;

internal sealed class DoctorStatusSql : IDoctorSql<DoctorStatusResponse>
{
    public string GetByKey => """
        SELECT * FROM "DoctorStatuses" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "DoctorStatuses" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "DoctorStatuses" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "DoctorStatuses"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
