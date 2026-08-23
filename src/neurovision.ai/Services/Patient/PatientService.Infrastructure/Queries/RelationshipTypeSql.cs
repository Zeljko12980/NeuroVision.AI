namespace PatientService.Infrastructure.Queries;

internal sealed class RelationshipTypeSql : IPatientSql<RelationshipTypeResponse>
{
    public string GetByKey => """
        SELECT * FROM "RelationshipTypes" WHERE "Code" = @Code OR LOWER("Name") = LOWER(@Name);
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "RelationshipTypes" WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "RelationshipTypes" WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "RelationshipTypes"
            WHERE (@Search IS NULL
             OR "Code" ILIKE '%' || @Search || '%'
             OR "Name" ILIKE '%' || @Search || '%'
             OR "Description" ILIKE '%' || @Search || '%')
            ORDER BY "Name"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
