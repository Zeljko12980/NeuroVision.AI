using Microsoft.AspNetCore.Http;

namespace LocationService.API.Contracts;

public static class FormFileExtensions
{
    public static async Task<byte[]?> ToBytesAsync(
        this IFormFile? file,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
            return null;

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        return stream.ToArray();
    }
}

public class CreateCountryForm
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime FoundingDate { get; set; }
    public int? CapitalSettlementCode { get; set; }
    public string? GovernmentTypeCode { get; set; }
    public int? CallingCode { get; set; }
    public IFormFile? Anthem { get; set; }
    public IFormFile? CoatOfArms { get; set; }
    public IFormFile? Flag { get; set; }
}

public class UpdateCountryForm
{
    public string Name { get; set; } = null!;
    public DateTime FoundingDate { get; set; }
    public int? CapitalSettlementCode { get; set; }
    public string? GovernmentTypeCode { get; set; }
    public int? CallingCode { get; set; }
    public IFormFile? Anthem { get; set; }
    public IFormFile? CoatOfArms { get; set; }
    public IFormFile? Flag { get; set; }
}
