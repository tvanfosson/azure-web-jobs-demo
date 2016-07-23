using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace WebJobDemo.Core.Data
{
    [ExcludeFromCodeCoverage]
    public class TransactionFactory : ITransactionFactory
    {
        public IDbTransaction Create(IDbConnection connection)
        {
            return connection.BeginTransaction();
        }

        public IDbTransaction Create(IDbConnection connection, IsolationLevel isolationLevel)
        {
            return connection.BeginTransaction(isolationLevel);
        }
    }
}