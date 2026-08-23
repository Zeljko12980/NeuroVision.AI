namespace DoctorService.Infrastructure.Queries;

internal sealed class DoctorLicenseHistorySql : IDoctorSql<DoctorLicenseHistoryResponse>
{
    public string GetByKey => """
        SELECT * FROM "DoctorLicenseHistories" WHERE "DoctorId" = @DoctorId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "DoctorLicenseHistories" WHERE "DoctorId" = @DoctorId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "DoctorLicenseHistories" WHERE (@Search IS NULL OR "LicenseNumber" ILIKE '%' || @Search || '%' OR COALESCE("LicenseAuthorityCode", '') ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "DoctorLicenseHistories"
            WHERE (@Search IS NULL OR "LicenseNumber" ILIKE '%' || @Search || '%' OR COALESCE("LicenseAuthorityCode", '') ILIKE '%' || @Search || '%')
            ORDER BY "DoctorId", "SequenceNumber" DESC
            LIMIT @PageSize OFFSET @Offset;
        """;
}
