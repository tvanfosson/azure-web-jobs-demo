using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WebApp.Configuration;
using WebApp.Data.Models;

namespace WebApp.Data
{
    public class AddSubscriptionCommand : DapperBase, IAddSubscriptionCommand
    {
        private const string InsertSql = @"INSERT (Id, FirstName, LastName, EmailAddress, SubscriptionKey) 
                                           OUTPUT inserted.Id, inserted.FirstName, inserted.LastName, inserted.EmailAddress, inserted.SubscriptionKey, inserted.CreatedOn, inserted.Version 
                                           VALUES ({id}, {firstName}, {lastName}, {emailAddress}, {subscriptionKey})";

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
                    var result = await connection.QueryAsync<Subscription>(InsertSql, new { id = Guid.NewGuid(), firstName, lastName, emailAddress, subscriptionKey = Guid.NewGuid() })
                                                 .ConfigureAwait(false);

                    var subscription = result.Single();

                    if (continueWith != null)
                    {
                        try
                        {
                            subscription = await continueWith(subscription);
                        }
                        catch
                        {
                            transaction.Rollback();
                            return null;
                        }
                    }

                    transaction.Commit();
                    return subscription;
                }
            }
        }
    }
}