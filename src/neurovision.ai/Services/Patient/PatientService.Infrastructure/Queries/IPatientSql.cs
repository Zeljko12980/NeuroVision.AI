namespace PatientService.Infrastructure.Queries;

internal interface IPatientSql<TResponse>
{
    string GetByKey { get; }
    string Exists { get; }
    string Count { get; }
    string GetPaged { get; }
}
