using DoctorService.Application.Common.Response;

namespace DoctorService.Infrastructure.Queries;

internal sealed class DoctorSql : IDoctorSql<DoctorResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "Doctors"
        WHERE "Id" = @Id;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "Doctors"
        WHERE "Id" = @Id;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "Doctors"
        WHERE (
            @Search IS NULL
            OR "FirstName" ILIKE '%' || @Search || '%'
            OR "LastName" ILIKE '%' || @Search || '%'
            OR "Email" ILIKE '%' || @Search || '%'
            OR "LicenseNumber" ILIKE '%' || @Search || '%'
        );
        """;

    public string GetPaged => """
        SELECT *
        FROM "Doctors"
        WHERE (
            @Search IS NULL
            OR "FirstName" ILIKE '%' || @Search || '%'
            OR "LastName" ILIKE '%' || @Search || '%'
            OR "Email" ILIKE '%' || @Search || '%'
            OR "LicenseNumber" ILIKE '%' || @Search || '%'
        )
        ORDER BY "LastName", "FirstName"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
