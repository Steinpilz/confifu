using System;
using System.Reflection;
using Confifu.Abstractions;
using Confifu.Abstractions.DependencyInjection;
using ServiceStack.WebHost.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ServiceStack.Confifu
{
    /// <summary>
    /// Consts holder
    /// </summary>
    public static class Const
    {
    }

    /// <summary>
    /// Strongly typed ServiceStack Confifu Config
    /// </summary>
    public class Config : LibraryConfig
    {
        /// <summary>
        /// Config based on App.Config
        /// </summary>
        public static Config Current => new Config(App.Config);

        public Config (IAppConfig appConfig) : base(appConfig, "ServiceStack:")
        {
        }
        
        /// <summary>
        /// ConfigProperty for ServiceStackConfig factory
        /// </summary>
        public ConfigProperty<Func<ServiceStackConfig>> ServiceStackConfig => Property(() => ServiceStackConfig);
    }

    /// <summary>
    /// Class holding extension methods for ServiceStack integration
    /// </summary>
    public static class AppConfigExtensions
    {
        /// <summary>
        /// User ServiceStack 
        /// </summary>
        /// <param name="appConfig"></param>
        /// <returns></returns>
        public static IAppConfig UseServiceStack(this IAppConfig appConfig)
        {
            return appConfig.EnsureInit();
        }

        /// <summary>
        /// Use ServiceStack with HttpAppHost
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="serviceName">ServiceStack serviceName</param>
        /// <param name="url">ServiceStack base url</param>
        /// <param name="assemblies">Assemblies with services</param>
        /// <returns></returns>
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

        /// <summary>
        /// Use ServiceStack with ASP.NET AppHost
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="serviceName">ServiceStack serviceName</param>
        /// <param name="assemblies">Assemblies with services</param>
        /// <returns></returns>
        public static IAppConfig UseWebAppHost(this IAppConfig appConfig, string serviceName,
            params Assembly[] assemblies)
        {
            if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            return appConfig
                .EnsureInit()
                .UseAppHost(() => new AppHostWebListener(serviceName, assemblies));
        }

        /// <summary>
        /// Use ServiceStack with custom AppHost
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="appHost">appHost factory</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Clear current appHost configuration
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <returns></returns>
        public static IAppConfig ClearAppHostConfiguration(this IAppConfig appConfig)
        {
            return appConfig
                .EnsureInit()
                .ConfigureStrong(config =>
            {
                config.ServiceStackConfig.Value().Config = null;
            });
        }

        /// <summary>
        /// Add appHost configuration action
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="configAction">configuration action</param>
        /// <returns></returns>
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

        /// <summary>
        /// Add ServiceStack plugin
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="plugin">ServiceStack plugin factory</param>
        /// <returns></returns>
        public static IAppConfig UseAppHostPlugin(this IAppConfig appConfig, Func<IPlugin> plugin)
        {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));
            return appConfig.AddAppHostConfiguration((appHost, config) =>
            {
                appHost.Plugins.Add(plugin());
            });
        }

        /// <summary>
        /// Add ServiceStack plugin by type
        /// </summary>
        /// <typeparam name="TPlugin">plugin type (must be registered in ServiceCollection)</typeparam>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <returns></returns>
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

                appConfig.AddAppHostConfiguration(
                    (appHost, c) => Default.ConfigureAppHost(appConfig, appHost, c));

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