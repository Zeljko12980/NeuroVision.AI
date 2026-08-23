namespace DoctorService.Infrastructure.Queries;

internal sealed class WorkingSlotSql : IDoctorSql<WorkingSlotResponse>
{
    private const string Columns = """
        "DoctorId",
        "DayOfWeek",
        "SequenceNumber",
        ("Start" - TIME '00:00') AS "Start",
        ("End" - TIME '00:00') AS "End",
        "ValidFrom",
        "ValidTo"
        """;

    public string GetByKey => $"""
        SELECT {Columns} FROM "WorkingSlots" WHERE "DoctorId" = @DoctorId AND "DayOfWeek" = @DayOfWeek AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*) FROM "WorkingSlots" WHERE "DoctorId" = @DoctorId AND "DayOfWeek" = @DayOfWeek AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*) FROM "WorkingSlots" WHERE (@Search IS NULL OR "DoctorId"::text ILIKE '%' || @Search || '%' OR "DayOfWeek"::text ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => $"""
        SELECT {Columns} FROM "WorkingSlots"
            WHERE (@Search IS NULL OR "DoctorId"::text ILIKE '%' || @Search || '%' OR "DayOfWeek"::text ILIKE '%' || @Search || '%')
            ORDER BY "DoctorId", "DayOfWeek", "SequenceNumber"
            LIMIT @PageSize OFFSET @Offset;
        """;
}
