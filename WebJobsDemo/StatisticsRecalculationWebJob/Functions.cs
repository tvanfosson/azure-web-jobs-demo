using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data;

namespace StatisticsRecalculationWebJob
{
    public class Functions
    {
        private readonly IApplicationSettings _settings;
        private readonly IConnectionFactory _connectionFactory;

        public Functions(IApplicationSettings settings, IConnectionFactory connectionFactory)
        {
            _settings = settings;
            _connectionFactory = connectionFactory;
        }

        [NoAutomaticTrigger]
        public async Task RecalculcateStatistics(TextWriter log)
        {
            await log.WriteLineAsync("Recalculating statistics");

            var command = new UpdateStatisticCommands(_settings, _connectionFactory);

            await command.Recalculate();

            await log.WriteLineAsync("Recalculation complete");
        }
    }
}
