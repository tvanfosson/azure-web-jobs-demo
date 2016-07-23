using System;
using System.Configuration;
using System.Linq;
using Autofac;
using Core.WebJobs;
using Microsoft.Azure.WebJobs;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data;

namespace StatisticsRecalculationWebJob
{
    class Program
    {
        public static void Main()
        {
            var settings = new ApplicationSettings("");

            if (!VerifyConfiguration(settings))
            {
                Console.ReadLine();
                return;
            }

            var container = RegisterDependencies();

            var config = new JobHostConfiguration
                         {
                             JobActivator = new AutofacActivator(container)
                         };

            var host = new JobHost(config);

            var tasks = typeof(Functions).GetMethods()
                                         .Where(m => m.GetCustomAttributes(typeof(NoAutomaticTriggerAttribute), false)
                                                      .Any());
            foreach (var method in tasks)
            {
                host.Call(method);
            }
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

        private static IContainer RegisterDependencies()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<Functions>()
                            .AsSelf()
                            .InstancePerLifetimeScope();

            containerBuilder.Register(c => new ApplicationSettings(""))
                            .As<IApplicationSettings>()
                            .InstancePerLifetimeScope();

            containerBuilder.RegisterType<SqlConnectionFactory>()
                            .As<IConnectionFactory>()
                            .InstancePerLifetimeScope();

            return containerBuilder.Build();
        }
    }
}
