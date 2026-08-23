namespace PatientService.Infrastructure.Queries;

internal sealed class PatientInsuranceHistorySql : IPatientSql<PatientInsuranceHistoryResponse>
{
    public string GetByKey => """
        SELECT * FROM "PatientInsuranceHistories" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "PatientInsuranceHistories" WHERE "PatientId" = @PatientId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "PatientInsuranceHistories" WHERE (@Search IS NULL OR "PayerCode" ILIKE '%' || @Search || '%' OR "PolicyNumber" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "PatientInsuranceHistories"
            WHERE (@Search IS NULL OR "PayerCode" ILIKE '%' || @Search || '%' OR "PolicyNumber" ILIKE '%' || @Search || '%')
            ORDER BY "PatientId", "SequenceNumber" DESC
            LIMIT @PageSize OFFSET @Offset;
        """;
}
