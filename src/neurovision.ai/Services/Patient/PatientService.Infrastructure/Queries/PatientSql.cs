namespace PatientService.Infrastructure.Queries;

internal sealed class PatientSql : IPatientSql<PatientResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "Patients"
        WHERE "Id" = @Id;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "Patients"
        WHERE "Id" = @Id;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "Patients"
        WHERE (
            @Search IS NULL
            OR "FirstName" ILIKE '%' || @Search || '%'
            OR "LastName" ILIKE '%' || @Search || '%'
            OR "Email" ILIKE '%' || @Search || '%'
            OR "NationalId" ILIKE '%' || @Search || '%'
            OR "Phone" ILIKE '%' || @Search || '%'
        );
        """;

    public string GetPaged => """
        SELECT *
        FROM "Patients"
        WHERE (
            @Search IS NULL
            OR "FirstName" ILIKE '%' || @Search || '%'
            OR "LastName" ILIKE '%' || @Search || '%'
            OR "Email" ILIKE '%' || @Search || '%'
            OR "NationalId" ILIKE '%' || @Search || '%'
            OR "Phone" ILIKE '%' || @Search || '%'
        )
        ORDER BY "LastName", "FirstName"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
