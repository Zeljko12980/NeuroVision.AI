namespace LocationService.Application.Common.Mappings;

public static class LocationMappings
{
    public static CountryResponse ToResponse(this Country entity) =>
        new()
        {
            Code = entity.Code,
            Name = entity.Name,
            FoundingDate = entity.FoundingDate,
            CapitalSettlementCode = entity.CapitalSettlementCode,
            CapitalSettlementName = entity.CapitalSettlement?.Name,
            GovernmentTypeCode = entity.GovernmentTypeCode,
            GovernmentTypeName = entity.GovernmentType?.Name,
            CallingCode = entity.CallingCode,
            Anthem = entity.Anthem,
            CoatOfArms = entity.CoatOfArms,
            Flag = entity.Flag,
            SettlementCount = entity.Settlements.Count,
            MunicipalityCount = entity.Municipalities.Count,
            HealthInstitutionCount = entity.HealthInstitutions.Count
        };

    public static GovernmentTypeResponse ToResponse(this GovernmentType entity) =>
        new()
        {
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description
        };

    public static RegionTypeResponse ToResponse(this RegionType entity) =>
        new()
        {
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description
        };

    public static HealthInstitutionTypeResponse ToResponse(this HealthInstitutionType entity) =>
        new()
        {
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description
        };

    public static SettlementResponse ToResponse(this Settlement entity) =>
        new()
        {
            CountryCode = entity.CountryCode,
            Code = entity.Code,
            Name = entity.Name,
            PostalCode = entity.PostalCode
        };

    public static MunicipalityResponse ToResponse(this Municipality entity) =>
        new()
        {
            CountryCode = entity.CountryCode,
            Code = entity.Code,
            Name = entity.Name,
            SeatSettlementCode = entity.SeatSettlementCode
        };

    public static LocalCommunityResponse ToResponse(this LocalCommunity entity) =>
        new()
        {
            CountryCode = entity.CountryCode,
            MunicipalityCode = entity.MunicipalityCode,
            Identifier = entity.Identifier,
            Name = entity.Name,
            OfficeSettlementCode = entity.OfficeSettlementCode
        };

    public static CapitalResponse ToResponse(this Capital entity) =>
        new()
        {
            CountryCode = entity.CountryCode,
            SettlementCode = entity.SettlementCode,
            SequenceNumber = entity.SequenceNumber,
            From = entity.From,
            To = entity.To
        };

    public static GovernmentHistoryResponse ToResponse(this GovernmentHistory entity) =>
        new()
        {
            CountryCode = entity.CountryCode,
            SequenceNumber = entity.SequenceNumber,
            GovernmentTypeCode = entity.GovernmentTypeCode,
            From = entity.From,
            To = entity.To
        };

    public static MunicipalitySettlementCoverageResponse ToResponse(this MunicipalitySettlementCoverage entity) =>
        new()
        {
            CountryCode = entity.CountryCode,
            MunicipalityCode = entity.MunicipalityCode,
            SettlementCode = entity.SettlementCode
        };

    public static LocalCommunityCoverageResponse ToResponse(this LocalCommunityCoverage entity) =>
        new()
        {
            CountryCode = entity.CountryCode,
            MunicipalityCode = entity.MunicipalityCode,
            LocalCommunityIdentifier = entity.LocalCommunityIdentifier,
            SettlementCode = entity.SettlementCode
        };

    public static RegionResponse ToResponse(this Region entity) =>
        new()
        {
            TypeCode = entity.TypeCode,
            Code = entity.Code,
            Name = entity.Name,
            BelongsToCountryCode = entity.BelongsToCountryCode,
            HeadquartersCountryCode = entity.HeadquartersCountryCode,
            AdministrativeSeatSettlementCode = entity.AdministrativeSeatSettlementCode
        };

    public static RegionSettlementCoverageResponse ToResponse(this RegionSettlementCoverage entity) =>
        new()
        {
            RegionTypeCode = entity.RegionTypeCode,
            RegionCode = entity.RegionCode,
            CountryCode = entity.CountryCode,
            SettlementCode = entity.SettlementCode
        };

    public static RegionCompositionResponse ToResponse(this RegionComposition entity) =>
        new()
        {
            ParentRegionTypeCode = entity.ParentRegionTypeCode,
            ParentRegionCode = entity.ParentRegionCode,
            MemberRegionTypeCode = entity.MemberRegionTypeCode,
            MemberRegionCode = entity.MemberRegionCode
        };

    public static CountryCompositionResponse ToResponse(this CountryComposition entity) =>
        new()
        {
            UnionCountryCode = entity.UnionCountryCode,
            MemberCountryCode = entity.MemberCountryCode,
            SequenceNumber = entity.SequenceNumber,
            From = entity.From,
            To = entity.To
        };

    public static LegalSuccessorResponse ToResponse(this LegalSuccessor entity) =>
        new()
        {
            SuccessorCountryCode = entity.SuccessorCountryCode,
            PredecessorCountryCode = entity.PredecessorCountryCode
        };

    public static HealthInstitutionResponse ToResponse(this HealthInstitution entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            TypeCode = entity.TypeCode,
            CountryCode = entity.CountryCode,
            SettlementCode = entity.SettlementCode,
            Address = entity.Address,
            BedCount = entity.BedCount,
            FoundingDate = entity.FoundingDate,
            Phone = entity.Phone
        };
}
