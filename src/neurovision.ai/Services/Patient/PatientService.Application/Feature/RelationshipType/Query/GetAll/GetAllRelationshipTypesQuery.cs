namespace PatientService.Application.Feature.RelationshipType.Query.GetAll;

public sealed record GetAllRelationshipTypesQuery(GetRelationshipTypesRequest Request)
    : IQuery<Result<PaginatedResult<RelationshipTypeResponse>>>;
