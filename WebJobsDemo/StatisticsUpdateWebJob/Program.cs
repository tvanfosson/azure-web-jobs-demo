using System;
using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using WebJobDemo.Core.Configuration;

namespace StatisticsUpdateWebJob
{
    class Program
    {
        private static string _serviceBusConnectionString;

        public static void Main()
        {
            var settings = new ApplicationSettings("");

            if (!VerifyConfiguration(settings))
            {
                Console.ReadLine();
                return;
            }

            _serviceBusConnectionString = settings.JobMessagesConnectionString;

            var config = new JobHostConfiguration();
            config.UseServiceBus(new ServiceBusConfiguration
            {
                ConnectionString = _serviceBusConnectionString,
            });

            var host = new JobHost(config);

            host.RunAndBlock();
        }

        private static bool VerifyConfiguration(IApplicationSettings settings)
        {
            var webJobsDashboard = ConfigurationManager.ConnectionStrings["AzureWebJobsDashboard"].ConnectionString;
            var webJobsStorage = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;

            var configOK = true;
            if (string.IsNullOrWhiteSpace(webJobsDashboard) || string.IsNullOrWhiteSpace(webJobsStorage))
            {
                configOK = false;
                Console.WriteLine("Please add the Azure Storage account credentials in App.config");
            }

            if (string.IsNullOrWhiteSpace(settings.JobMessagesConnectionString))
            {
                configOK = false;
                Console.WriteLine("Please add your Service Bus connection string in App.config");
            }

            return configOK;
        }
    }
}
