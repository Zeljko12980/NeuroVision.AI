namespace PatientService.Application.Feature.Language.Query.GetAll;

public sealed record GetAllLanguagesQuery(GetLanguagesRequest Request)
    : IQuery<Result<PaginatedResult<LanguageResponse>>>;
