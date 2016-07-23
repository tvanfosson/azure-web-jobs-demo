using System.Data;

namespace WebJobDemo.Core.Data
{
    public interface IConnectionFactory
    {
        IDbConnection Create(string connectionString);
    }
}
