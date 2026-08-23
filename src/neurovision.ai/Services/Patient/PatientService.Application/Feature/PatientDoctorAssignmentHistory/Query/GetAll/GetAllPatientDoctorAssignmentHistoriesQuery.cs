namespace PatientService.Application.Feature.PatientDoctorAssignmentHistory.Query.GetAll;

public sealed record GetAllPatientDoctorAssignmentHistoriesQuery(GetPatientDoctorAssignmentHistoriesRequest Request)
    : IQuery<Result<PaginatedResult<PatientDoctorAssignmentHistoryResponse>>>;
