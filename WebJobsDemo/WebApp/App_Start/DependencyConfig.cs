using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using WebApp.Configuration;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.App_Start
{
    public static class DependencyConfig
    {
        public static void Register(Action<IDependencyResolver> setResolver)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ApplicationSettings>()
                   .As<IApplicationSettings>()
                   .SingleInstance();

            builder.RegisterType<TransactionFactory>()
                   .As<ITransactionFactory>()
                   .SingleInstance();

            builder.RegisterType<SqlConnectionFactory>()
                   .As<IConnectionFactory>()
                   .InstancePerRequest();

            builder.RegisterType<OfflineProcessingService>()
                   .As<IOfflineProcessingService>()
                   .InstancePerRequest();

            builder.RegisterType<AddSubscriptionCommand>()
                   .As<IAddSubscriptionCommand>()
                   .InstancePerRequest();

            builder.RegisterType<SubscriptionQuery>()
                   .As<ISubscriptionQuery>()
                   .InstancePerRequest();

            builder.RegisterType<SubscriptionService>()
                   .AsSelf()
                   .InstancePerRequest();

            setResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }
}