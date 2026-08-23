namespace PatientService.Application.Feature.PatientEmergencyContact.Query.GetAll;

public sealed record GetAllPatientEmergencyContactsQuery(GetPatientEmergencyContactsRequest Request)
    : IQuery<Result<PaginatedResult<PatientEmergencyContactResponse>>>;
