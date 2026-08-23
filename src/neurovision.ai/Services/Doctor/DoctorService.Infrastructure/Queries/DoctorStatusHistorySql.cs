namespace DoctorService.Infrastructure.Queries;

internal sealed class DoctorStatusHistorySql : IDoctorSql<DoctorStatusHistoryResponse>
{
    public string GetByKey => """
        SELECT * FROM "DoctorStatusHistories" WHERE "DoctorId" = @DoctorId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "DoctorStatusHistories" WHERE "DoctorId" = @DoctorId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "DoctorStatusHistories" WHERE (@Search IS NULL OR "StatusCode" ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "DoctorStatusHistories"
            WHERE (@Search IS NULL OR "StatusCode" ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%')
            ORDER BY "DoctorId", "SequenceNumber" DESC
            LIMIT @PageSize OFFSET @Offset;
        """;
}
