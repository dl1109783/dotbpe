using DotBPE.Rpc.Client;
using DotBPE.Rpc.Client.RouterPolicy;
using DotBPE.Rpc.Protocol;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peach;
using Peach.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DotBPE.Rpc.ServiceDiscovery;
using Microsoft.Extensions.Configuration;

namespace DotBPE.Rpc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotBPE(this IServiceCollection services)
        {
            //add common
            services.AddLogging();
            services.AddOptions();
            services.AddAmpProtocol();
            services.AddDefaultImpl();
            return services;
        }
        public static IServiceCollection AddDefaultRegisterService(this IServiceCollection services)
        {
             services.TryAddSingleton<IServiceRegister, DefaultServiceRegister>();
             return services;
        }

        public static IServiceCollection BindService<TService>(this IServiceCollection services)
            where  TService:class,IServiceActor<AmpMessage>
        {
            return services.AddSingleton<IServiceActor<AmpMessage>, TService>();
        }

        public static IServiceCollection BindServices(this IServiceCollection services,
            Action<ServiceActorCollection> serviceConfigureAction)
        {
            var list = new ServiceActorCollection();
            serviceConfigureAction(list);


            var actorTypes = list.GetTypeAll();
            var instances = list.GetInstanceAll();

            foreach (var actorType in actorTypes)
            {
                services.AddSingleton(typeof(IServiceActor<AmpMessage>), actorType);
            }
            foreach (var actor in instances)
            {
                services.AddSingleton(actor);
            }

            return services;
        }


        public static IServiceCollection ScanBindServices(this IServiceCollection services,IConfiguration configuration
            ,string dllPrefix ,params string[] categories)
        {
            string BaseDirectory = Internal.Environment.GetAppBasePath();

            var dllFiles = Directory.GetFiles(string.Concat(BaseDirectory,""), $"{dllPrefix}.dll");
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in dllFiles)
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }

            //扫描注册所有的ServiceRegistry
            //扫描注册所有的ServiceActor
            var serviceRegistryType = typeof(IServiceDependencyRegistry);
            var serviceActorType = typeof(AbsServiceActor);
            List<Type> registryTypes = new List<Type>();
            List<Type> actorTypes = new List<Type>();
            foreach (Assembly a in assemblies)
            {
                //Console.WriteLine(a.FullName);
                foreach (var t in a.GetTypes())
                {
                    if (serviceRegistryType.IsAssignableFrom(t) && t.IsClass) //t 实现了某接口
                    {
                        registryTypes.Add(t);
                    }
                    else if (t.IsSubclassOf(serviceActorType) && !t.IsAbstract)
                    {
                        actorTypes.Add(t);
                    }
                }
            }

            if (registryTypes.Count > 0) //注册依赖
            {
                registryTypes.ForEach(r => ServiceActorDescriptor.ServiceDependencyRegistry(configuration, services, r));
            }
            if (actorTypes.Count > 0) //注册服务
            {
                ServiceActorDescriptor.AddServiceActor(services, actorTypes,categories);
            }
            return services;
        }

        #region Route Policy

        public static IServiceCollection AddDiscoveryServiceRouter(this IServiceCollection services)
        {
            services.Remove(ServiceDescriptor.Singleton(typeof(IServiceRouter)));
            return services.AddSingleton<IServiceRouter, DiscoveryServiceRouter>();

        }

        public static IServiceCollection AddRandomPolicy(this IServiceCollection services)
        {
            services.Remove(ServiceDescriptor.Singleton(typeof(IRouterPolicy)));
            return services.AddSingleton<IRouterPolicy, RandomPolicy>();
        }


        public static IServiceCollection AddWeightedRoundRobinPolicy(this IServiceCollection services)
        {
            services.Remove(ServiceDescriptor.Singleton(typeof(IRouterPolicy)));
            return services.AddSingleton<IRouterPolicy, WeightedRoundRobinPolicy>();
        }

        #endregion



        #region  Private Method
        private static IServiceCollection AddAmpProtocol(this IServiceCollection services)
        {
            services.AddSingleton<IProtocol<AmpMessage>, AmpProtocol>();
            services.AddSingleton<ISocketClient<AmpMessage>, RpcSocketClient>();
            services.AddSingleton<ISocketService<AmpMessage>, AmpRpcService>();
            return services;
        }

        private static IServiceCollection AddDefaultImpl(this IServiceCollection services)
        {
            //sever
            services.TryAddSingleton<IServiceActorLocator<AmpMessage>, DefaultServiceActorLocator>();
            services.TryAddSingleton<IServerMessageHandler<AmpMessage>, DefaultServerMessageHandler>();

            //client
            services.TryAddSingleton<IRpcClient<AmpMessage>, DefaultRpcClient>();
            services.TryAddSingleton<ICallInvoker, DefaultCallInvoker>();
            services.TryAddSingleton<IClientMessageHandler<AmpMessage>, DefaultClientMessageHandler>();
            services.TryAddSingleton<IRouterPolicy, RoundrobinPolicy>();
            services.TryAddSingleton<IServiceRouter, DefaultServiceRouter>();
            services.TryAddSingleton<IClientAuditLoggerFactory, DefaultClientAuditLoggerFactory>();
            services.TryAddSingleton<IRequestAuditLoggerFactory, DefaultRequestAuditLoggerFactory>();
            services.TryAddSingleton<ITransportFactory<AmpMessage>, DefaultTransportFactory>();


            return services;
        }


        #endregion




    }
}
