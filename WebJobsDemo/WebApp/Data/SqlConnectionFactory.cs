using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace WebApp.Data
{
    [ExcludeFromCodeCoverage]
    public class SqlConnectionFactory : IConnectionFactory
    {
        public IDbConnection Create(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}