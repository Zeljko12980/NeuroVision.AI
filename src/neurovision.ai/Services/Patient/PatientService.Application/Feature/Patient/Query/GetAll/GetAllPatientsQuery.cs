namespace PatientService.Application.Feature.Patient.Query.GetAll;

public sealed record GetAllPatientsQuery(GetPatientsRequest Request)
    : IQuery<Result<PaginatedResult<PatientResponse>>>;
