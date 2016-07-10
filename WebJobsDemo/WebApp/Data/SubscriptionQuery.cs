using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WebApp.Configuration;
using WebApp.Data.Models;

namespace WebApp.Data
{
    public class SubscriptionQuery : DapperBase, ISubscriptionQuery
    {
        private const string SelectSql = "SELECT * FROM Subscriptions";

        public SubscriptionQuery(IApplicationSettings settings, IConnectionFactory connectionFactory)
            : base(settings, connectionFactory)
        {
        }

        private string GetSql(SqlBuilder builder)
        {
            return builder.AddTemplate(SelectSql + "/**where**/").RawSql;
        }

        public async Task<ICollection<Subscription>> GetSubscriptions()
        {
            using (var connection = CreateConnection())
            {
                var subscriptions = await connection.QueryAsync<Subscription>(SelectSql);

                return subscriptions.ToList();
            }
        }

        public async Task<Subscription> GetSubscriptionById(Guid id)
        {
            var builder = new SqlBuilder().Where("Id = {id}", new { id });

            var sql = GetSql(builder);

            using (var connection = CreateConnection())
            {
                var subscription = await connection.QueryAsync<Subscription>(sql);

                return subscription.FirstOrDefault();
            }
        }

        public async Task<Subscription> GetSubscriptionByEmailAddressAndSubscriptionKey(string emailAddress, Guid subscriptionKey)
        {
            var builder = new SqlBuilder().Where("EmailAddress = {emailAddress}", new { emailAddress })
                                          .Where("SubscriptionKey = {subscriptionKey}", subscriptionKey);

            var sql = GetSql(builder);

            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<Subscription>(sql);

                var subscription = result.FirstOrDefault();

                return subscription;
            }
        }
    }
}