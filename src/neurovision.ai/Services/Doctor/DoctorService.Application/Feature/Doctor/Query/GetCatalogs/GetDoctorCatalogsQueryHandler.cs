namespace DoctorService.Application.Feature.Doctor.Query.GetCatalogs;

public sealed class GetDoctorCatalogsQueryHandler
    : IQueryHandler<GetDoctorCatalogsQuery, Result<DoctorCatalogsResponse>>
{
    private const int CatalogPageSize = 100;
    private static readonly object CatalogQuery = new
    {
        Search = (string?)null,
        PageSize = CatalogPageSize,
        Offset = 0
    };

    private readonly IDoctorReadStore<SpecializationResponse> specializations;
    private readonly IDoctorReadStore<LanguageResponse> languages;
    private readonly IDoctorReadStore<DegreeTypeResponse> degreeTypes;
    private readonly IDoctorReadStore<LicenseAuthorityResponse> licenseAuthorities;

    public GetDoctorCatalogsQueryHandler(
        IDoctorReadStore<SpecializationResponse> specializations,
        IDoctorReadStore<LanguageResponse> languages,
        IDoctorReadStore<DegreeTypeResponse> degreeTypes,
        IDoctorReadStore<LicenseAuthorityResponse> licenseAuthorities)
    {
        this.specializations = specializations;
        this.languages = languages;
        this.degreeTypes = degreeTypes;
        this.licenseAuthorities = licenseAuthorities;
    }

    public async Task<Result<DoctorCatalogsResponse>> Handle(
        GetDoctorCatalogsQuery query,
        CancellationToken cancellationToken)
    {
        var specializationItems = await specializations.GetPagedAsync(CatalogQuery, cancellationToken);
        var languageItems = await languages.GetPagedAsync(CatalogQuery, cancellationToken);
        var degreeItems = await degreeTypes.GetPagedAsync(CatalogQuery, cancellationToken);
        var authorityItems = await licenseAuthorities.GetPagedAsync(CatalogQuery, cancellationToken);

        return Result<DoctorCatalogsResponse>.Ok(
            new DoctorCatalogsResponse
            {
                Specializations = specializationItems,
                Languages = languageItems,
                DegreeTypes = degreeItems,
                LicenseAuthorities = authorityItems
            });
    }
}
