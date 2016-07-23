using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WebJobDemo.Core.Configuration;

namespace WebJobDemo.Core.Data
{
    public class UpdateStatisticCommands : DapperBase, IUpdateStatisticCommands
    {
        private const string UpdateConfirmedSql = "UPDATE DomainStatistics SET [Confirmed] = [Confirmed] + 1, LastUpdated = @lastUpdated WHERE Domain = @domain";
        private const string UpdateCountSql = "UPDATE DomainStatistics SET [Count] = [Count] + 1, LastUpdated = @lastUpdated WHERE Domain = @domain";
        private const string InsertSql = "INSERT INTO DomainStatistics (Domain,Count,LastUpdated) VALUES (@domain,1,getutcdate())";

        public UpdateStatisticCommands(IApplicationSettings settings, IConnectionFactory connectionFactory)
            : base(settings, connectionFactory)
        {
        }

        public async Task AddOrUpdateDomainCount(string domain, IDbConnection connection, IDbTransaction transaction)
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

        public async Task AddConfirmation(string domain)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var count = (await connection.QueryAsync<int>(CountDomainStatsSql, new {domain}, transaction)).FirstOrDefault();

                        if (count == 0)
                        {
                            throw new InvalidOperationException("Cannot confirm a domain that has yet to be counted");
                        }

                        await connection.ExecuteAsync(UpdateConfirmedSql, new {domain, lastUpdated = DateTime.UtcNow}, transaction);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public async Task Recalculate()
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync("RecalculateDomainStatistics", commandType: CommandType.StoredProcedure);
            }
        }
    }
}
