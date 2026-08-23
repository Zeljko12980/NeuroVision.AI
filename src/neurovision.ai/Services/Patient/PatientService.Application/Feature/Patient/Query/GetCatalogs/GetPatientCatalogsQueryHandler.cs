namespace PatientService.Application.Feature.Patient.Query.GetCatalogs;

public sealed class GetPatientCatalogsQueryHandler
    : IQueryHandler<GetPatientCatalogsQuery, Result<PatientCatalogsResponse>>
{
    private const int CatalogPageSize = 100;
    private static readonly object CatalogQuery = new
    {
        Search = (string?)null,
        PageSize = CatalogPageSize,
        Offset = 0
    };

    private readonly IPatientReadStore<PatientStatusResponse> statuses;
    private readonly IPatientReadStore<GenderResponse> genders;
    private readonly IPatientReadStore<BloodTypeResponse> bloodTypes;
    private readonly IPatientReadStore<LanguageResponse> languages;
    private readonly IPatientReadStore<AllergyResponse> allergies;
    private readonly IPatientReadStore<ConditionResponse> conditions;
    private readonly IPatientReadStore<InsurancePayerResponse> insurancePayers;
    private readonly IPatientReadStore<RelationshipTypeResponse> relationshipTypes;
    private readonly IPatientReadStore<ConsentTypeResponse> consentTypes;

    public GetPatientCatalogsQueryHandler(
        IPatientReadStore<PatientStatusResponse> statuses,
        IPatientReadStore<GenderResponse> genders,
        IPatientReadStore<BloodTypeResponse> bloodTypes,
        IPatientReadStore<LanguageResponse> languages,
        IPatientReadStore<AllergyResponse> allergies,
        IPatientReadStore<ConditionResponse> conditions,
        IPatientReadStore<InsurancePayerResponse> insurancePayers,
        IPatientReadStore<RelationshipTypeResponse> relationshipTypes,
        IPatientReadStore<ConsentTypeResponse> consentTypes)
    {
        this.statuses = statuses;
        this.genders = genders;
        this.bloodTypes = bloodTypes;
        this.languages = languages;
        this.allergies = allergies;
        this.conditions = conditions;
        this.insurancePayers = insurancePayers;
        this.relationshipTypes = relationshipTypes;
        this.consentTypes = consentTypes;
    }

    public async Task<Result<PatientCatalogsResponse>> Handle(
        GetPatientCatalogsQuery query,
        CancellationToken cancellationToken)
    {
        return Result<PatientCatalogsResponse>.Ok(
            new PatientCatalogsResponse
            {
                Statuses = await statuses.GetPagedAsync(CatalogQuery, cancellationToken),
                Genders = await genders.GetPagedAsync(CatalogQuery, cancellationToken),
                BloodTypes = await bloodTypes.GetPagedAsync(CatalogQuery, cancellationToken),
                Languages = await languages.GetPagedAsync(CatalogQuery, cancellationToken),
                Allergies = await allergies.GetPagedAsync(CatalogQuery, cancellationToken),
                Conditions = await conditions.GetPagedAsync(CatalogQuery, cancellationToken),
                InsurancePayers = await insurancePayers.GetPagedAsync(CatalogQuery, cancellationToken),
                RelationshipTypes = await relationshipTypes.GetPagedAsync(CatalogQuery, cancellationToken),
                ConsentTypes = await consentTypes.GetPagedAsync(CatalogQuery, cancellationToken)
            });
    }
}
