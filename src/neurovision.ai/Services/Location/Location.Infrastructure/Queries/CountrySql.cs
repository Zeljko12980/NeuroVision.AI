namespace LocationService.Infrastructure.Queries;

internal sealed class CountrySql : ILocationSql<CountryResponse>
{
    public string GetByKey => """
        SELECT
            c."Code",
            c."Name",
            c."FoundingDate",
            c."CapitalSettlementCode",
            s."Name" AS "CapitalSettlementName",
            c."GovernmentTypeCode",
            g."Name" AS "GovernmentTypeName",
            c."CallingCode",
            c."Anthem",
            c."CoatOfArms",
            c."Flag",
            (SELECT COUNT(*) FROM "Settlements" st WHERE st."CountryCode" = c."Code") AS "SettlementCount",
            (SELECT COUNT(*) FROM "Municipalities" m WHERE m."CountryCode" = c."Code") AS "MunicipalityCount",
            (SELECT COUNT(*) FROM "HealthInstitutions" h WHERE h."CountryCode" = c."Code") AS "HealthInstitutionCount"
        FROM "Countries" c
        LEFT JOIN "GovernmentTypes" g ON g."Code" = c."GovernmentTypeCode"
        LEFT JOIN "Settlements" s ON s."CountryCode" = c."Code" AND s."Code" = c."CapitalSettlementCode"
        WHERE c."Code" = @Code;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "Countries"
        WHERE "Code" = @Code;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "Countries" c
        WHERE (@Search IS NULL OR c."Name" ILIKE '%' || @Search || '%')
          AND (@GovernmentTypeCode IS NULL OR c."GovernmentTypeCode" = @GovernmentTypeCode)
          AND (@IncludeCapital = FALSE OR c."CapitalSettlementCode" IS NOT NULL);
        """;

    public string GetPaged => """
        SELECT
            c."Code",
            c."Name",
            c."FoundingDate",
            c."CapitalSettlementCode",
            s."Name" AS "CapitalSettlementName",
            c."GovernmentTypeCode",
            g."Name" AS "GovernmentTypeName",
            c."CallingCode",
            c."Anthem",
            c."CoatOfArms",
            c."Flag",
            (SELECT COUNT(*) FROM "Settlements" st WHERE st."CountryCode" = c."Code") AS "SettlementCount",
            (SELECT COUNT(*) FROM "Municipalities" m WHERE m."CountryCode" = c."Code") AS "MunicipalityCount",
            (SELECT COUNT(*) FROM "HealthInstitutions" h WHERE h."CountryCode" = c."Code") AS "HealthInstitutionCount"
        FROM "Countries" c
        LEFT JOIN "GovernmentTypes" g ON g."Code" = c."GovernmentTypeCode"
        LEFT JOIN "Settlements" s ON s."CountryCode" = c."Code" AND s."Code" = c."CapitalSettlementCode"
        WHERE (@Search IS NULL OR c."Name" ILIKE '%' || @Search || '%')
          AND (@GovernmentTypeCode IS NULL OR c."GovernmentTypeCode" = @GovernmentTypeCode)
          AND (@IncludeCapital = FALSE OR c."CapitalSettlementCode" IS NOT NULL)
        ORDER BY c."Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
