namespace DoctorService.Application.Feature.DoctorLanguage.Query.GetAll;

public sealed record GetAllDoctorLanguagesQuery(GetDoctorLanguagesRequest Request)
    : IQuery<Result<PaginatedResult<LanguageResponse>>>;
