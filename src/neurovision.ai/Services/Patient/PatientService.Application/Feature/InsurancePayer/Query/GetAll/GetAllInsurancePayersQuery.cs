namespace PatientService.Application.Feature.InsurancePayer.Query.GetAll;

public sealed record GetAllInsurancePayersQuery(GetInsurancePayersRequest Request)
    : IQuery<Result<PaginatedResult<InsurancePayerResponse>>>;
