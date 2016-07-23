using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data.Models;

namespace WebJobDemo.Core.Data
{
    public class SubscriptionQuery : DapperBase, ISubscriptionQuery
    {
        private const string SelectSubscriptionsSql = "SELECT * FROM Subscriptions";
        private const string SelectStatisticsSql = "SELECT * FROM DomainStatistics";

        public SubscriptionQuery(IApplicationSettings settings, IConnectionFactory connectionFactory)
            : base(settings, connectionFactory)
        {
        }

        private string GetSql(SqlBuilder builder)
        {
            return builder.AddTemplate(SelectSubscriptionsSql + " /**where**/").RawSql;
        }

        public async Task<ICollection<Subscription>> GetSubscriptions()
        {
            using (var connection = CreateConnection())
            {
                var subscriptions = await connection.QueryAsync<Subscription>(SelectSubscriptionsSql);

                return subscriptions.ToList();
            }
        }

        public async Task<Subscription> GetSubscriptionById(Guid id)
        {
            var builder = new SqlBuilder().Where("Id = @id");

            var sql = GetSql(builder);

            using (var connection = CreateConnection())
            {
                var subscription = await connection.QueryAsync<Subscription>(sql, new { id });

                return subscription.FirstOrDefault();
            }
        }

        public async Task<Subscription> GetSubscriptionByEmailAddressAndSubscriptionKey(string emailAddress, Guid subscriptionKey)
        {
            var builder = new SqlBuilder().Where("EmailAddress = @emailAddress")
                                          .Where("SubscriptionKey = @subscriptionKey");

            var sql = GetSql(builder);

            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<Subscription>(sql, new { emailAddress, subscriptionKey });

                var subscription = result.FirstOrDefault();

                return subscription;
            }
        }

        public async Task<ICollection<Subscription>> GetSubscriptionsWithMissingConfirmations()
        {
            var builder = new SqlBuilder().Where("ConfirmationSentOn IS NULL");

            var sql = GetSql(builder);

            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<Subscription>(sql);

                return result.ToList();
            }
        }

        public async Task<ICollection<DomainStatistic>> GetStatistics()
        {
            using (var connection = CreateConnection())
            {
                var stats = await connection.QueryAsync<DomainStatistic>(SelectStatisticsSql);

                return stats.ToList();
            }
        }
    }
}