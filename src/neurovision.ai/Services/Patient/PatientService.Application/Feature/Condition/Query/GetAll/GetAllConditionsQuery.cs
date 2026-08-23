namespace PatientService.Application.Feature.Condition.Query.GetAll;

public sealed record GetAllConditionsQuery(GetConditionsRequest Request)
    : IQuery<Result<PaginatedResult<ConditionResponse>>>;
