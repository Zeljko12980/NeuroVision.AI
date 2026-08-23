namespace DoctorService.Infrastructure.Queries;

internal sealed class LicenseAuthoritySql : IDoctorSql<LicenseAuthorityResponse>
{
    public string GetByKey => """
        SELECT * FROM "LicenseAuthorities" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "LicenseAuthorities" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "LicenseAuthorities" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "LicenseAuthorities"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
