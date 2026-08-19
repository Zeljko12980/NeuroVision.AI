namespace LocationService.Domain.Entities;

public class HealthInstitution
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string TypeCode { get; private set; } = null!;
    public string CountryCode { get; private set; } = null!;
    public int SettlementCode { get; private set; }
    public string? Address { get; private set; }
    public int? BedCount { get; private set; }
    public DateTime? FoundingDate { get; private set; }
    public string? Phone { get; private set; }

    public HealthInstitutionType Type { get; private set; } = null!;
    public Country Country { get; private set; } = null!;
    public Settlement Settlement { get; private set; } = null!;

    private HealthInstitution()
    {
    }

    public static HealthInstitution Create(
        string name,
        string typeCode,
        string countryCode,
        int settlementCode,
        string? address = null,
        int? bedCount = null,
        DateTime? foundingDate = null,
        string? phone = null)
    {
        if (settlementCode <= 0)
            throw new ArgumentException("Settlement code must be greater than zero.", nameof(settlementCode));

        if (bedCount is < 0)
            throw new ArgumentException("Bed count cannot be negative.", nameof(bedCount));

        return new HealthInstitution
        {
            Name = Guard.NotEmpty(name, nameof(name)),
            TypeCode = Guard.NotEmpty(typeCode, nameof(typeCode)),
            CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant(),
            SettlementCode = settlementCode,
            Address = address,
            BedCount = bedCount,
            FoundingDate = foundingDate,
            Phone = phone
        };
    }

    public void Update(
        string name,
        string typeCode,
        string countryCode,
        int settlementCode,
        string? address,
        int? bedCount,
        DateTime? foundingDate,
        string? phone)
    {
        if (settlementCode <= 0)
            throw new ArgumentException("Settlement code must be greater than zero.", nameof(settlementCode));

        if (bedCount is < 0)
            throw new ArgumentException("Bed count cannot be negative.", nameof(bedCount));

        Name = Guard.NotEmpty(name, nameof(name));
        TypeCode = Guard.NotEmpty(typeCode, nameof(typeCode));
        CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant();
        SettlementCode = settlementCode;
        Address = address;
        BedCount = bedCount;
        FoundingDate = foundingDate;
        Phone = phone;
    }
}
