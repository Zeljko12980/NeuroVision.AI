using Dapper;

namespace BuildingBlocks.Dapper
{
    public class SqlQueryExecutor
     : ISqlQueryExecutor
    {
        private readonly ISqlConnectionFactory _factory;

        static SqlQueryExecutor()
        {
            DapperTypeHandlers.EnsureRegistered();
        }

        public SqlQueryExecutor(
            ISqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object? param = null)
        {
            using var connection = _factory.CreateConnection();

            return await connection.QueryAsync<T>(
                sql,
                param);
        }

        public async Task<T?> QuerySingleAsync<T>(
            string sql,
            object? param = null)
        {
            using var connection = _factory.CreateConnection();

            return await connection.QuerySingleOrDefaultAsync<T>(
                sql,
                param);
        }

        public async Task<int> ExecuteAsync(
            string sql,
            object? param = null)
        {
            using var connection = _factory.CreateConnection();

            return await connection.ExecuteAsync(
                sql,
                param);
        }
    }
}
