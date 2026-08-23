namespace PatientService.Application.Feature.Patient.Query.GetByKey;

public sealed record GetPatientByKeyQuery(Guid Id) : IQuery<Result<PatientResponse>>;
