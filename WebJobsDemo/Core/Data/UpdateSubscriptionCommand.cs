using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data.Models;

namespace WebJobDemo.Core.Data
{
    public class UpdateSubscriptionCommand : DapperBase, IUpdateSubscriptionCommand
    {
        private const string UpdateSql = @"UPDATE [dbo].[Subscriptions]
                                           SET [FirstName] = @FirstName,
                                               [LastName] = @LastName,
                                               [EmailAddress] = @EmailAddress,
                                               [CreatedOn] = @CreatedOn,
                                               [ConfirmationSentOn] = @ConfirmationSentOn,
                                               [Confirmed] = @Confirmed
                                           WHERE Id = @Id";

        public UpdateSubscriptionCommand(IApplicationSettings settings, IConnectionFactory connectionFactory)
            : base(settings, connectionFactory)
        {
        }

        public async Task Update(Subscription subscription, Func<Subscription, IDbConnection, IDbTransaction, Task> continueWith = null)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(UpdateSql, subscription, transaction);

                        if (continueWith != null)
                        {
                            await continueWith(subscription, connection, transaction);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
