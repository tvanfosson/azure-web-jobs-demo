using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data.Models;

namespace WebJobDemo.Core.Data
{
    public class AddSubscriptionCommand : DapperBase, IAddSubscriptionCommand
    {
        private const string InsertSql = @"INSERT INTO [dbo].[Subscriptions] (Id, FirstName, LastName, EmailAddress, SubscriptionKey) 
                                           OUTPUT inserted.Id, inserted.FirstName, inserted.LastName, inserted.EmailAddress, inserted.SubscriptionKey, inserted.CreatedOn, inserted.Version 
                                           VALUES (@id, @firstname, @lastName, @emailAddress, @subscriptionKey)";

        private readonly ITransactionFactory _transactionFactory;

        public AddSubscriptionCommand(IApplicationSettings settings, IConnectionFactory connectionFactory, ITransactionFactory transactionFactory)
            : base(settings, connectionFactory)
        {
            _transactionFactory = transactionFactory;
        }

        public async Task<Subscription> Add(string firstName, string lastName, string emailAddress, Func<Subscription, Task<Subscription>> continueWith)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = _transactionFactory.Create(connection))
                {
                    try
                    {
                        var result = await connection.QueryAsync<Subscription>(InsertSql, new { id = Guid.NewGuid(), firstName, lastName, emailAddress, subscriptionKey = Guid.NewGuid() }, transaction)
                                                 .ConfigureAwait(false);

                        var subscription = result.Single();

                        if (continueWith != null)
                        {
                            subscription = await continueWith(subscription);
                        }

                        transaction.Commit();

                        return subscription;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return null;
                    }
                }
            }
        }
    }
}