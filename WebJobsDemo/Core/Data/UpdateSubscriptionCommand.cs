using System;
using System.Data;
using System.Linq;
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

        private const string UpdateCountSql = "UPDATE DomainStatistics SET [Count] = [Count] + 1, LastUpdated = @lastUpdated WHERE Domain = @domain";
        private const string InsertSql = "INSERT INTO DomainStatistics (Domain,Count,LastUpdated) VALUES (@domain,1,getutcdate())";

        public UpdateSubscriptionCommand(IApplicationSettings settings, IConnectionFactory connectionFactory)
            : base(settings, connectionFactory)
        {
        }

        public async Task Update(Subscription subscription, Func<Subscription, Task> continueWith = null)
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

                        await AddToDomainCount(subscription.GetDomain(), connection, transaction);

                        if (continueWith != null)
                        {
                            await continueWith(subscription);
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

        private static async Task AddToDomainCount(string domain, IDbConnection connection = null, IDbTransaction transaction = null)
        {

            var count = (await connection.QueryAsync<int>(CountDomainStatsSql, new { domain }, transaction)).FirstOrDefault();

            if (count == 0)
            {
                await connection.ExecuteAsync(InsertSql, new { domain }, transaction);
            }
            else
            {
                await connection.ExecuteAsync(UpdateCountSql, new { domain, lastUpdated = DateTime.UtcNow }, transaction);
            }
        }
    }
}
