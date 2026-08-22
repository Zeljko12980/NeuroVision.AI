namespace DoctorService.Application.Feature.Doctor.Query.GetAll;

public sealed record GetAllDoctorsQuery(GetDoctorsRequest Request)
    : IQuery<Result<PaginatedResult<DoctorResponse>>>;
