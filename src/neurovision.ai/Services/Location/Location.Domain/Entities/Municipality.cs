namespace LocationService.Domain.Entities;

public class Municipality
{
    public string CountryCode { get; private set; } = null!;
    public int Code { get; private set; }
    public string Name { get; private set; } = null!;
    public int? SeatSettlementCode { get; private set; }

    public Country Country { get; private set; } = null!;
    public Settlement? SeatSettlement { get; private set; }

    public ICollection<LocalCommunity> LocalCommunities { get; private set; } = new List<LocalCommunity>();
    public ICollection<MunicipalitySettlementCoverage> Settlements { get; private set; } = new List<MunicipalitySettlementCoverage>();

    private Municipality()
    {
    }

    public static Municipality Create(string countryCode, int code, string name, int? seatSettlementCode = null)
    {
        if (code <= 0)
            throw new ArgumentException("Municipality code must be greater than zero.", nameof(code));

        return new Municipality
        {
            CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant(),
            Code = code,
            Name = Guard.NotEmpty(name, nameof(name)),
            SeatSettlementCode = seatSettlementCode
        };
    }

    public void Update(string name, int? seatSettlementCode)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        SeatSettlementCode = seatSettlementCode;
    }
}
