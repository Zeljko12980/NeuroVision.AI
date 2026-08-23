using BuildingBlocks.Dapper;
using PatientService.Application.Common.Interfaces;

namespace PatientService.Infrastructure.Persistence;

internal sealed class SequenceStore : ISequenceStore
{
    private readonly ISqlQueryExecutor sql;

    public SequenceStore(ISqlQueryExecutor sql)
    {
        this.sql = sql;
    }

    public async Task<int> NextAsync(
        string table,
        string sequenceColumn,
        CancellationToken cancellationToken,
        params (string Column, object Value)[] scope)
    {
        var where = scope.Length == 0
            ? "TRUE"
            : string.Join(" AND ", scope.Select((item, index) => $"\"{item.Column}\" = @p{index}"));

        var parameters = new Dictionary<string, object>();
        for (var index = 0; index < scope.Length; index++)
        {
            parameters[$"p{index}"] = scope[index].Value;
        }

        var query = $@"SELECT COALESCE(MAX(""{sequenceColumn}""), 0) + 1
            FROM ""{table}""
            WHERE {where};";

        return await sql.QuerySingleAsync<int>(query, parameters);
    }
}
