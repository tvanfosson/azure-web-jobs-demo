using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data;
using WebJobDemo.Core.Data.Models;

namespace StatisticsUpdateWebJob
{
    public class Functions
    {
        private readonly IApplicationSettings _settings;
        private readonly IConnectionFactory _connectionFactory;

        public Functions()
            : this(new ApplicationSettings(""), new SqlConnectionFactory())
        {
        }

        public Functions(IApplicationSettings settings, IConnectionFactory connectionFactory)
        {
            _settings = settings;
            _connectionFactory = connectionFactory;
        }

        public async Task UpdateStats(
            [ServiceBusTrigger(ApplicationSettings.ConfirmationTopic, ApplicationSettings.StatsSubscription)] BrokeredMessage message,
            TextWriter log)
        {
            var subscription = message.GetBody<Subscription>();

            var domain = subscription.GetDomain();

            await log.WriteLineAsync($"Updating stats for {domain} due to entry for {subscription.EmailAddress}");

            var update = new UpdateStatisticCommands(_settings, _connectionFactory);

            await update.AddConfirmation(domain);

            await log.WriteLineAsync("Stats updated");
        }
    }
}
