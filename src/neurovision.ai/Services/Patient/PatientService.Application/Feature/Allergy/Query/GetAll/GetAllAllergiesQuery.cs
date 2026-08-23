namespace PatientService.Application.Feature.Allergy.Query.GetAll;

public sealed record GetAllAllergiesQuery(GetAllergiesRequest Request)
    : IQuery<Result<PaginatedResult<AllergyResponse>>>;
