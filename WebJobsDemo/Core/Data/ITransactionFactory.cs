using System.Data;

namespace WebJobDemo.Core.Data
{
    public interface ITransactionFactory
    {
        IDbTransaction Create(IDbConnection connection);
        IDbTransaction Create(IDbConnection connection, IsolationLevel isolationLevel);
    }
}
