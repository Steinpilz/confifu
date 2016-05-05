using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confifu.Abstractions.DependencyInjection
{
    public static class AppConfigExtensions
    {
        public const string ServiceCollectionKey = "ServiceCollection";
        public const string ServiceProviderKey = "ServiceProvider";

        public static T Get<T>(this IAppConfig appConfig, string key)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (key == null) throw new ArgumentNullException(nameof(key));
            var config = appConfig[key];
            if (!(config is T))
                return default(T);
            return (T) config;
        }

        public static IServiceCollection GetServiceCollection(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.Get<IServiceCollection>(ServiceCollectionKey);
        }

        public static void SetServiceCollection(this IAppConfig appConfig, IServiceCollection serviceCollection)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            appConfig[ServiceCollectionKey] = serviceCollection;
        }

        public static IServiceProvider GetServiceProvider(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.Get<IServiceProvider>(ServiceProviderKey);
        }

        public static IAppConfig RegisterServices(this IAppConfig appConfig, Action<IServiceCollection> action)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            var serviceCollection = appConfig.GetServiceCollection();
            action(serviceCollection);
            return appConfig;
        }

        public static void SetServiceProvider(this IAppConfig appConfig, IServiceProvider serviceProvider)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            appConfig[ServiceProviderKey] = serviceProvider;
        }
    }
}