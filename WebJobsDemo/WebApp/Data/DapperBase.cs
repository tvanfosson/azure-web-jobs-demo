using System.Data;
using WebApp.Configuration;

namespace WebApp.Data
{
    public abstract class DapperBase
    {
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