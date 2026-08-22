namespace DoctorService.Infrastructure.Queries;

internal interface IDoctorSql<TResponse>
{
    string GetByKey { get; }
    string Exists { get; }
    string Count { get; }
    string GetPaged { get; }
}
