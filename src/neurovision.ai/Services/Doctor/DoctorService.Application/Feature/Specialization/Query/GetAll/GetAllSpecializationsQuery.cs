namespace DoctorService.Application.Feature.Specialization.Query.GetAll;

public sealed record GetAllSpecializationsQuery(GetSpecializationsRequest Request)
    : IQuery<Result<PaginatedResult<SpecializationResponse>>>;
