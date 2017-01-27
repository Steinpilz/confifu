using System;
using System.Collections.Generic;
using System.Reflection;
using Confifu.Abstractions;
using Confifu.Abstractions.DependencyInjection;
using ServiceStack.WebHost.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Confifu.LibraryConfig;

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
        /// User ServiceStack with HttpAppHost (using configured default ServiceHost name and Service Assemblies list)
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="url">ServiceStack base url</param>
        /// <returns></returns>
        public static IAppConfig UseHttpAppHost(this IAppConfig appConfig, string url)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (url == null) throw new ArgumentNullException(nameof(url));

            return appConfig
                .EnsureInit()
                .UseAppHost(() =>
                {
                    var serviceStackConfig = appConfig.Strong().ServiceStackConfig.Value();

                    return new AppHostHttpListener(serviceStackConfig.ServiceHostName, url,
                        serviceStackConfig.ServiceHostAssemblies.ToArray());
                });
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
        /// Use ServiceStack with ASP.NET AppHost (using configured default ServiceHost name and Service Assemblies list)
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <returns></returns>
        public static IAppConfig UseWebAppHost(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.EnsureInit()
                .UseAppHost(() =>
                {
                    var serviceStackConfig = appConfig.Strong().ServiceStackConfig.Value();

                    return new AppHostWebListener(serviceStackConfig.ServiceHostName,
                        serviceStackConfig.ServiceHostAssemblies.ToArray());
                });
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
        /// Configure ServiceStackConfig instance using <para>action</para>
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="action">Action used to configure</param>
        /// <returns></returns>
        public static IAppConfig ConfigureServiceStackConfig(this IAppConfig appConfig,
            Action<ServiceStackConfig> action)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (action == null) throw new ArgumentNullException(nameof(action));

            return appConfig.EnsureInit()
                .ConfigureStrong(config =>
                {
                    action(config.ServiceStackConfig.Value());
                });
        }

        /// <summary>
        /// Sets ServiceStackHost default name (will be passed to new IAppHost instance)
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="name">ServiceHost name</param>
        /// <returns></returns>
        public static IAppConfig SetServiceHostName(this IAppConfig appConfig, string name)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (name == null) throw new ArgumentNullException(nameof(name));
            return appConfig.ConfigureServiceStackConfig(c => c.ServiceHostName = name);
        }

        /// <summary>
        /// Adds <para>assembliues</para> to the default list assemblies which will be passed 
        /// to IAppHost instance
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="assemblies">List of assemblies with services</param>
        /// <returns></returns>
        public static IAppConfig AddServiceHostAssemblies(this IAppConfig appConfig, List<Assembly> assemblies)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            return appConfig.ConfigureServiceStackConfig(config =>
            {
                config.ServiceHostAssemblies.AddRange(assemblies);
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