using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace WebJobDemo.Core.Data
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