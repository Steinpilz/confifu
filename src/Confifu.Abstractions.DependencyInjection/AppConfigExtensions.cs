using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confifu.Abstractions.DependencyInjection
{
    /// <summary>
    /// Class holding IAppConfig extension methods to integrate
    /// Microsoft.Extensions.DependencyInject.Abstractions 
    /// <see cref="https://github.com/aspnet/DependencyInjection"/>
    /// </summary>
    public static class AppConfigExtensions
    {
        /// <summary>
        /// ServiceCollection predefined key
        /// </summary>
        public const string ServiceCollectionKey = "ServiceCollection";

        /// <summary>
        /// ServiceProvider predefined key
        /// </summary>
        public const string ServiceProviderKey = "ServiceProvider";

        /// <summary>
        /// Get ServiceCollection from <para>appConfig</para> using ServiceCollection predefined key
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <returns>IServiceCollection instance</returns>
        public static IServiceCollection GetServiceCollection(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.Get<IServiceCollection>(ServiceCollectionKey);
        }

        /// <summary>
        /// Set ServiceCollection to <para>appConfig</para> using ServiceCollection predefined key
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="serviceCollection">IServiceCollection instance</param>
        public static void SetServiceCollection(this IAppConfig appConfig, IServiceCollection serviceCollection)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            appConfig[ServiceCollectionKey] = serviceCollection;
        }

        /// <summary>
        /// Get ServiceProvider from <para>appConfig</para> using ServiceProvider predefined key
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <returns>IServiceProvider instance</returns>
        public static IServiceProvider GetServiceProvider(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.Get<IServiceProvider>(ServiceProviderKey);
        }

        /// <summary>
        /// Set ServiceProvider to <para>appConfig</para> using ServiceProvider predefined key
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="serviceProvider">IServiceCollection instance</param>
        public static void SetServiceProvider(this IAppConfig appConfig, IServiceProvider serviceProvider)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            appConfig[ServiceProviderKey] = serviceProvider;
        }

        /// <summary>
        /// Call given <para>action</para> with ServiceCollection of given <para>appConfig</para>
        /// as the first argument
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="action"></param>
        /// <returns>given <para>IAppConfig</para> instance</returns>
        public static IAppConfig RegisterServices(this IAppConfig appConfig, Action<IServiceCollection> action)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            var serviceCollection = appConfig.GetServiceCollection();
            action(serviceCollection);
            return appConfig;
        }

        /// <summary>
        /// Register Common Services (IAppConfig, IConfigVariables) to ServiceCollection of given <para>appConfig</para>.
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <returns>given <para>IAppConfig</para> instance</returns>
        public static IAppConfig RegisterCommonServices(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.RegisterServices(sc =>
            {
                sc.AddTransient<IAppConfig>(sp => appConfig);

                var configVariables = appConfig.GetConfigVariables();
                sc.AddTransient<IConfigVariables>(sp => configVariables);
            });
        }
    }
}