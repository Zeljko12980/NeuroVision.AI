namespace LocationService.Infrastructure.Queries;

internal interface ILocationSql<TResponse>
{
    string GetByKey { get; }
    string Exists { get; }
    string Count { get; }
    string GetPaged { get; }
}
