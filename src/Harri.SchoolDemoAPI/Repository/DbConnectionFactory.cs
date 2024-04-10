using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace Harri.SchoolDemoAPI.Repository
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string? _connectionString;

        public DbConnectionFactory(string? connectionString)
        {
            if (connectionString != null)
            {
                _connectionString = connectionString;
            }
        }

        public IDbConnection GetConnection()
        {
            if (_connectionString != null)
            {
                return new SqlConnection(_connectionString);
            }

            throw new System.Exception("_connectionString is null");
        }
    }
}
