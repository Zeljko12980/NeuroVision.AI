using System.Data;

namespace BuildingBlocks.Dapper
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
