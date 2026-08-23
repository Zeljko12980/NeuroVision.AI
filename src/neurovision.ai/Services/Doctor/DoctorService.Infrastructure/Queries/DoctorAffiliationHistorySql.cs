namespace DoctorService.Infrastructure.Queries;

internal sealed class DoctorAffiliationHistorySql : IDoctorSql<DoctorAffiliationHistoryResponse>
{
    public string GetByKey => """
        SELECT * FROM "DoctorAffiliationHistories" WHERE "DoctorId" = @DoctorId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "DoctorAffiliationHistories" WHERE "DoctorId" = @DoctorId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "DoctorAffiliationHistories" WHERE (@Search IS NULL OR "InstitutionName" ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "DoctorAffiliationHistories"
            WHERE (@Search IS NULL OR "InstitutionName" ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%')
            ORDER BY "DoctorId", "SequenceNumber" DESC
            LIMIT @PageSize OFFSET @Offset;
        """;
}
