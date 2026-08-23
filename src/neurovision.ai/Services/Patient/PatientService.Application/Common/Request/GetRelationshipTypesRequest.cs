namespace PatientService.Application.Common.Request;

public record GetRelationshipTypesRequest(string? Search) : PaginationRequest;
