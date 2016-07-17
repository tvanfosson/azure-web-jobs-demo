using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using MoreLinq;
using WebApp.App_Start;
using WebApp.Services;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data;

namespace WebApp
{
    public static class DependencyConfig
    {
        private static readonly Type _profileType = typeof (Profile);

        private static readonly ISet<Type> _profileTypes = typeof (MapperConfig).Assembly
                                                                                .GetTypes()
                                                                                .Where(t => _profileType.IsAssignableFrom(t))
                                                                                .ToHashSet();

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

            builder.RegisterType<UpdateSubscriptionCommand>()
                   .As<IUpdateSubscriptionCommand>()
                   .InstancePerRequest();

            builder.RegisterType<SubscriptionQuery>()
                   .As<ISubscriptionQuery>()
                   .InstancePerRequest();

            builder.RegisterType<SubscriptionService>()
                   .As<ISubscriptionService>()
                   .InstancePerRequest();

            builder.RegisterAssemblyTypes(typeof (MapperConfig).Assembly)
                   .Where(t => _profileTypes.Contains(t))
                   .InstancePerRequest();

            builder.Register(c =>
                    {
                        var profiles = _profileTypes.Select(t => c.Resolve(t) as Profile);
                        var mapper = MapperConfig.Register(profiles);
                        return mapper;
                    })
                   .As<IMapper>()
                   .InstancePerRequest();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterModule(new AutofacWebTypesModule());

            setResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }
}