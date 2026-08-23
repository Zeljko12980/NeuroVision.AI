namespace PatientService.Infrastructure.Queries;

internal sealed class PatientAllergyCoverageSql : IPatientSql<PatientAllergyCoverageResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientAllergyCoverages" WHERE "PatientId" = @PatientId AND "AllergyCode" = @AllergyCode;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientAllergyCoverages" WHERE "PatientId" = @PatientId AND "AllergyCode" = @AllergyCode;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientAllergyCoverages" WHERE (@Search IS NULL OR "AllergyCode" ILIKE '%' || @Search || '%' OR COALESCE("Note", '') ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientAllergyCoverages"
            WHERE (@Search IS NULL OR "AllergyCode" ILIKE '%' || @Search || '%' OR COALESCE("Note", '') ILIKE '%' || @Search || '%')
            ORDER BY "PatientId", "AllergyCode"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
