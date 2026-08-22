using BuildingBlocks.Dapper;
using PatientService.Application.Common.Interfaces;

namespace PatientService.Infrastructure.Persistence;

internal sealed class PatientReadStore<TResponse> : IPatientReadStore<TResponse>
{
    private readonly ISqlQueryExecutor _sql;
    private readonly IPatientSql<TResponse> _queries;

    public PatientReadStore(ISqlQueryExecutor sql, IPatientSql<TResponse> queries)
    {
        _sql = sql;
        this._queries = queries;
    }

    public async Task<TResponse?> GetByKeyAsync(
        object parameters,
        CancellationToken cancellationToken = default)
    {
        return await _sql.QuerySingleAsync<TResponse>(_queries.GetByKey, parameters);
    }

    public async Task<IReadOnlyList<TResponse>> GetPagedAsync(
        object parameters,
        CancellationToken cancellationToken = default)
    {
        var items = await _sql.QueryAsync<TResponse>(_queries.GetPaged, parameters);
        return items.ToList();
    }

    public async Task<int> CountAsync(
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        return await _sql.QuerySingleAsync<int>(_queries.Count, parameters);
    }

    public async Task<bool> ExistsAsync(
        object parameters,
        CancellationToken cancellationToken = default)
    {
        var count = await _sql.QuerySingleAsync<int>(_queries.Exists, parameters);
        return count > 0;
    }
}
