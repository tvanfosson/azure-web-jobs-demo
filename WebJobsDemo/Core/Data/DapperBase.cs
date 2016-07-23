using System.Data;
using WebJobDemo.Core.Configuration;

namespace WebJobDemo.Core.Data
{
    public abstract class DapperBase
    {
        protected const string CountDomainStatsSql = "SELECT COUNT(*) as [Count] FROM DomainStatistics WHERE Domain = @domain";

        private readonly IConnectionFactory _connectionFactory;
        private readonly IApplicationSettings _settings;

        protected DapperBase(IApplicationSettings settings, IConnectionFactory connectionFactory)
        {
            _settings = settings;
            _connectionFactory = connectionFactory;
        }

        protected IDbConnection CreateConnection()
        {
            return _connectionFactory.Create(_settings.WebJobsDemoConnectionString);
        }

    }
}