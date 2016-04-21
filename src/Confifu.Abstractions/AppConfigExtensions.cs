using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confifu.Abstractions
{
    public static class AppConfigExtensions
    {
        public const string ConfigVariablesKey = "ConfigVariables";
        public const string ServiceCollectionKey = "ServiceCollection";
        public const string ServiceProviderKey = "ServiceProvider";

        public static T Get<T>(this IAppConfig appConfig, string key)
        {
            var config = appConfig[key];
            if (!(config is T))
                return default(T);
            return (T) config;
        }

        public static IServiceCollection GetServiceCollection(this IAppConfig appConfig)
        {
            return appConfig.Get<IServiceCollection>(ServiceCollectionKey);
        }

        public static void SetServiceCollection(this IAppConfig appConfig, IServiceCollection serviceCollection)
        {
            appConfig[ServiceCollectionKey] = serviceCollection;
        }

        public static IServiceProvider GetServiceProvider(this IAppConfig appConfig)
        {
            return appConfig.Get<IServiceProvider>(ServiceProviderKey);
        }

        public static void SetServiceProvider(this IAppConfig appConfig, IServiceProvider serviceProvider)
        {
            appConfig[ServiceProviderKey] = serviceProvider;
        }

        public static IConfigVariables GetConfigVariables(this IAppConfig appConfig)
        {
            return appConfig.Get<IConfigVariables>(ConfigVariablesKey);
        }

        public static void SetConfigVariables(this IAppConfig appConfig,
            IConfigVariables configVariables)
        {
            appConfig[ConfigVariablesKey] = configVariables;
        }
    }
}