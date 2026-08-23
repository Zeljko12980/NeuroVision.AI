namespace DoctorService.Infrastructure.Queries;

internal sealed class SpecializationSql : IDoctorSql<SpecializationResponse>
{
    public string GetByKey => """
        SELECT * FROM "Specializations" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "Specializations" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "Specializations" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "Specializations"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
