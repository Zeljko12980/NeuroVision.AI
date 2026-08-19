namespace LocationService.Domain.Entities;

public class Country
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public DateTime FoundingDate { get; private set; }
    public int? CapitalSettlementCode { get; private set; }
    public string? GovernmentTypeCode { get; private set; }
    public int? CallingCode { get; private set; }
    public byte[]? Anthem { get; private set; }
    public byte[]? CoatOfArms { get; private set; }
    public byte[]? Flag { get; private set; }

    public GovernmentType? GovernmentType { get; private set; }
    public Settlement? CapitalSettlement { get; private set; }

    public ICollection<Settlement> Settlements { get; private set; } = new List<Settlement>();
    public ICollection<Municipality> Municipalities { get; private set; } = new List<Municipality>();
    public ICollection<Capital> CapitalHistory { get; private set; } = new List<Capital>();
    public ICollection<GovernmentHistory> GovernmentHistory { get; private set; } = new List<GovernmentHistory>();
    public ICollection<Region> HomeRegions { get; private set; } = new List<Region>();
    public ICollection<LegalSuccessor> SuccessorOf { get; private set; } = new List<LegalSuccessor>();
    public ICollection<LegalSuccessor> PredecessorOf { get; private set; } = new List<LegalSuccessor>();
    public ICollection<CountryComposition> MemberOfUnions { get; private set; } = new List<CountryComposition>();
    public ICollection<CountryComposition> UnionMembers { get; private set; } = new List<CountryComposition>();
    public ICollection<HealthInstitution> HealthInstitutions { get; private set; } = new List<HealthInstitution>();

    private Country()
    {
    }

    public static Country Create(
        string code,
        string name,
        DateTime foundingDate,
        int? capitalSettlementCode = null,
        string? governmentTypeCode = null,
        int? callingCode = null,
        byte[]? anthem = null,
        byte[]? coatOfArms = null,
        byte[]? flag = null)
    {
        return new Country
        {
            Code = Guard.NotEmpty(code, nameof(code)).ToUpperInvariant(),
            Name = Guard.NotEmpty(name, nameof(name)),
            FoundingDate = foundingDate,
            CapitalSettlementCode = capitalSettlementCode,
            GovernmentTypeCode = string.IsNullOrWhiteSpace(governmentTypeCode) ? null : governmentTypeCode.Trim(),
            CallingCode = callingCode,
            Anthem = anthem,
            CoatOfArms = coatOfArms,
            Flag = flag
        };
    }

    public void Update(
        string name,
        DateTime foundingDate,
        int? capitalSettlementCode,
        string? governmentTypeCode,
        int? callingCode,
        byte[]? anthem,
        byte[]? coatOfArms,
        byte[]? flag)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        FoundingDate = foundingDate;
        CapitalSettlementCode = capitalSettlementCode;
        GovernmentTypeCode = string.IsNullOrWhiteSpace(governmentTypeCode) ? null : governmentTypeCode.Trim();
        CallingCode = callingCode;

        if (anthem is not null)
            Anthem = anthem;

        if (coatOfArms is not null)
            CoatOfArms = coatOfArms;

        if (flag is not null)
            Flag = flag;
    }

    public void SetCapitalSettlement(int? settlementCode) =>
        CapitalSettlementCode = settlementCode;
}
