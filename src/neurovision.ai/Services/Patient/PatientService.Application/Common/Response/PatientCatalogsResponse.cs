namespace PatientService.Application.Common.Response;

public class CatalogItemResponse
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class PatientStatusResponse : CatalogItemResponse;
public class GenderResponse : CatalogItemResponse;
public class BloodTypeResponse : CatalogItemResponse;
public class LanguageResponse : CatalogItemResponse;
public class AllergyResponse : CatalogItemResponse;
public class ConditionResponse : CatalogItemResponse;
public class InsurancePayerResponse : CatalogItemResponse;
public class RelationshipTypeResponse : CatalogItemResponse;
public class ConsentTypeResponse : CatalogItemResponse;

public class PatientCatalogsResponse
{
    public IReadOnlyList<PatientStatusResponse> Statuses { get; init; } = [];
    public IReadOnlyList<GenderResponse> Genders { get; init; } = [];
    public IReadOnlyList<BloodTypeResponse> BloodTypes { get; init; } = [];
    public IReadOnlyList<LanguageResponse> Languages { get; init; } = [];
    public IReadOnlyList<AllergyResponse> Allergies { get; init; } = [];
    public IReadOnlyList<ConditionResponse> Conditions { get; init; } = [];
    public IReadOnlyList<InsurancePayerResponse> InsurancePayers { get; init; } = [];
    public IReadOnlyList<RelationshipTypeResponse> RelationshipTypes { get; init; } = [];
    public IReadOnlyList<ConsentTypeResponse> ConsentTypes { get; init; } = [];
}
