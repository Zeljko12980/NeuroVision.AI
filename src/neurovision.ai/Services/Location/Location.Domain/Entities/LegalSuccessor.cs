namespace LocationService.Domain.Entities;

public class LegalSuccessor
{
    public string SuccessorCountryCode { get; private set; } = null!;
    public string PredecessorCountryCode { get; private set; } = null!;

    public Country SuccessorCountry { get; private set; } = null!;
    public Country PredecessorCountry { get; private set; } = null!;

    private LegalSuccessor()
    {
    }

    public static LegalSuccessor Create(string successorCountryCode, string predecessorCountryCode)
    {
        var successor = Guard.NotEmpty(successorCountryCode, nameof(successorCountryCode)).ToUpperInvariant();
        var predecessor = Guard.NotEmpty(predecessorCountryCode, nameof(predecessorCountryCode)).ToUpperInvariant();

        if (successor == predecessor)
            throw new ArgumentException("Successor and predecessor countries must be different.");

        return new LegalSuccessor
        {
            SuccessorCountryCode = successor,
            PredecessorCountryCode = predecessor
        };
    }
}
