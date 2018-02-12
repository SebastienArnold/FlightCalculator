using System.Data;
using Microsoft.Data.Sqlite;

namespace FlightCalculator.DataAccessLayer
{
    public class SqlLiteProvider : IDbProvider
    {
        private readonly string _connectionString;

        public SqlLiteProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
