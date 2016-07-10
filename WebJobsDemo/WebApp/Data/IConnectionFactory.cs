using System.Data;

namespace WebApp.Data
{
    public interface IConnectionFactory
    {
        IDbConnection Create(string connectionString);
    }
}
