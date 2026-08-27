using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Domain.Entities;

public sealed class ClinicalCatalogItem
{
    public ClinicalCatalogCategory Category { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private ClinicalCatalogItem() { }

    public static ClinicalCatalogItem Create(
        ClinicalCatalogCategory category,
        string code,
        string name,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        return new ClinicalCatalogItem
        {
            Category = category,
            Code = code.Trim(),
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
        };
    }
}
