namespace DoctorService.Infrastructure.Queries;

internal sealed class DoctorReviewSql : IDoctorSql<DoctorReviewResponse>
{
    public string GetByKey => """
        SELECT * FROM "DoctorReviews" WHERE "DoctorId" = @DoctorId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "DoctorReviews" WHERE "DoctorId" = @DoctorId AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "DoctorReviews" WHERE (@Search IS NULL OR COALESCE("Comment", '') ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT * FROM "DoctorReviews"
            WHERE (@Search IS NULL OR COALESCE("Comment", '') ILIKE '%' || @Search || '%' OR "DoctorId"::text ILIKE '%' || @Search || '%')
            ORDER BY "DoctorId", "SequenceNumber" DESC
            LIMIT @PageSize OFFSET @Offset;
        """;
}
