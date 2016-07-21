using System.Configuration;
using Autofac;
using Core.WebJobs;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Diagnostics;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using WebJobDemo.Core.Configuration;

namespace SendWebHooksWebJob
{
    public class Program
    {

        static void Main(string[] args)
        {
            var settings = new ApplicationSettings("");
            var listener = ConfigurationManager.ConnectionStrings["WebHookListener"].ConnectionString;

            var config = new JobHostConfiguration
            {
                StorageConnectionString = listener,
                JobActivator = new AutofacActivator(RegisterDependencies())
            };

            var serviceBusConnectionString = settings.JobMessagesConnectionString;

            config.UseServiceBus(new ServiceBusConfiguration
            {
                ConnectionString = serviceBusConnectionString,
            });

            var host = new JobHost(config);

            host.RunAndBlock();
        }

        private static IContainer RegisterDependencies()
        {
            var containerBuilder = new ContainerBuilder();


            containerBuilder.Register(c =>
                            {
                                var logger = new TraceLogger();

                                var store = AzureWebHookStore.CreateStore(logger);

                                var sender = new DataflowWebHookSender(logger);

                                var manager = new WebHookManager(store, sender, logger);

                                return manager;
                            })
                            .As<IWebHookManager>()
                            .InstancePerLifetimeScope();

            containerBuilder.RegisterType<Functions>()
                            .AsSelf()
                            .InstancePerLifetimeScope();

            return containerBuilder.Build();
        }
    }
}
