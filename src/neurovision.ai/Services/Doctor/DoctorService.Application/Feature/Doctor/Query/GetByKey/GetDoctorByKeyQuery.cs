namespace DoctorService.Application.Feature.Doctor.Query.GetByKey;

public sealed record GetDoctorByKeyQuery(Guid Id) : IQuery<Result<DoctorResponse>>;
