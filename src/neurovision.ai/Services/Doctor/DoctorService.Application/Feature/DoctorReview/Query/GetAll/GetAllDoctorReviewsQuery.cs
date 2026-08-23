namespace DoctorService.Application.Feature.DoctorReview.Query.GetAll;

public sealed record GetAllDoctorReviewsQuery(GetDoctorReviewsRequest Request)
    : IQuery<Result<PaginatedResult<DoctorReviewResponse>>>;
