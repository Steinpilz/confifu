using System;
using System.Reflection;
using Confifu.Abstractions;
using Confifu.Abstractions.DependencyInjection;
using ServiceStack.WebHost.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ServiceStack.Confifu
{
    public static class Const
    {
    }

    public class Config : LibraryConfig
    {
        public static Config Current => new Config(App.Config);

        public Config (IAppConfig appConfig) : base(appConfig, "ServiceStack:")
        {
        }
        
        public ConfigProperty<Func<ServiceStackConfig>> ServiceStackConfig => Property(() => ServiceStackConfig);
    }

    public static class AppConfigExtensions
    {
        public static IAppConfig UseServiceStack(this IAppConfig appConfig)
        {
            return appConfig.EnsureInit();
        }

        public static IAppConfig UseHttpAppHost(this IAppConfig appConfig, string serviceName, string url,
            params Assembly[] assemblies)
        {
            if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            return appConfig
                .EnsureInit()
                .UseAppHost(() => new AppHostHttpListener(serviceName, url, assemblies));
        }

        public static IAppConfig UseWebAppHost(this IAppConfig appConfig, string serviceName,
            params Assembly[] assemblies)
        {
            if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            return appConfig
                .EnsureInit()
                .UseAppHost(() => new AppHostWebListener(serviceName, assemblies));
        }

        public static IAppConfig UseAppHost(this IAppConfig appConfig, Func<IRunnableAppHost> appHost)
        {
            if (appHost == null) throw new ArgumentNullException(nameof(appHost));

            return appConfig
                .EnsureInit()
                .ConfigureStrong(config =>
                {
                    config.ServiceStackConfig.Value().AppHost = appHost;
                });
        }
        
        public static IAppConfig ClearAppHostConfiguration(this IAppConfig appConfig)
        {
            return appConfig
                .EnsureInit()
                .ConfigureStrong(config =>
            {
                config.ServiceStackConfig.Value().Config = null;
            });
        }

        public static IAppConfig AddAppHostConfiguration(this IAppConfig appConfig, Action<IAppHost, EndpointHostConfig> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            return appConfig
                .EnsureInit()
                .ConfigureStrong(config =>
            {
                config.ServiceStackConfig.Value().AddConfigAction(configAction);
            });
        }

        public static IAppConfig UseAppHostPlugin(this IAppConfig appConfig, Func<IPlugin> plugin)
        {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));
            return appConfig.AddAppHostConfiguration((appHost, config) =>
            {
                appHost.Plugins.Add(plugin());
            });
        }

        public static IAppConfig UseAppHostPlugin<TPlugin>(this IAppConfig appConfig)
            where TPlugin : IPlugin
        {
            return appConfig
                .EnsureInit()
                .UseAppHostPlugin(() => appConfig.GetServiceProvider().GetService<TPlugin>());
        }

        internal static IAppConfig EnsureInit(this IAppConfig appConfig)
        {
            return appConfig.ConfigureStrong(config =>
            {
                if (config.Initialized)
                    return;
                config.Initialized = true;

                var serviceStackConfig = new ServiceStackConfig();
                config.ServiceStackConfig.Value = () => serviceStackConfig;

                appConfig
                    .WrapAppRunner(runner => () =>
                    {
                        runner?.Invoke();
                        new ServiceStackAppRunner(config.ServiceStackConfig.Value()).Run();
                    });
            });
        }

        internal static IAppConfig ConfigureStrong(this IAppConfig appConfig, Action<Config> configureAction)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));

            var config = appConfig.Strong();
            configureAction(config);
            return appConfig;
        }

        internal static Config Strong(this IAppConfig appConfig)
        {
            return new Config(appConfig);
        }
    }
}  