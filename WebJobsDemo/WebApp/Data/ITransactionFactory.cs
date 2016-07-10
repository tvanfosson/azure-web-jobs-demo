using System.Data;

namespace WebApp.Data
{
    public interface ITransactionFactory
    {
        IDbTransaction Create(IDbConnection connection);
        IDbTransaction Create(IDbConnection connection, IsolationLevel isolationLevel);
    }
}
