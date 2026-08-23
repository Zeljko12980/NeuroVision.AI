namespace PatientService.Application.Feature.ConsentType.Query.GetAll;

public sealed record GetAllConsentTypesQuery(GetConsentTypesRequest Request)
    : IQuery<Result<PaginatedResult<ConsentTypeResponse>>>;
