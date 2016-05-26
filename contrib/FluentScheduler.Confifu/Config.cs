using System;
using Confifu.Abstractions;

namespace FluentScheduler.Confifu
{
    /// <summary>
    /// Constants holder
    /// </summary>
    public static class Const
    {
    }

    /// <summary>
    /// Strongly typeg FluentScheduler Confifu Config
    /// </summary>
    public class Config : LibraryConfig
    {
        public static Config Current => new Config(App.Config);

        public Config(IAppConfig appConfig) : base(appConfig,  "FluentScheduler:")
        {
        }

        /// <summary>
        /// ConfigProperty for Common Registry factory
        /// </summary>
        public ConfigProperty<Func<Registry>> Registry => Property(() => Registry);

        /// <summary>
        /// ConfigProperty for Regirstry Config Action
        /// </summary>
        public ConfigProperty<Action<Registry>> RegistryConfigFunc => Property(() => RegistryConfigFunc);
    }


    /// <summary>
    /// Class holding IAppConfig extension methods for FluentScheduler integration
    /// </summary>
    public static class AppConfigExtensions
    {
        /// <summary>
        /// Configure IAppConfig to use FluentScheduler. <para>configureFunc</para> will be added 
        /// to existing FluentScheduler configuration. All configured jobs will be run in AppRunner,
        /// so no more actions needed.
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="configureFunc">Registry configuration action</param>
        /// <returns></returns>
        public static IAppConfig UseFluentScheduler(this IAppConfig appConfig, 
            Action<Registry> configureFunc)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (configureFunc == null) throw new ArgumentNullException(nameof(configureFunc));

            return appConfig
                .EnsureInit()
                .ConfigureStrong(config =>
            {
                var currentConfigFunc = config.RegistryConfigFunc.Value;
                config.RegistryConfigFunc.Value = registry =>
                {
                    currentConfigFunc?.Invoke(registry);
                    configureFunc(registry);
                };
            });
        }

        internal static IAppConfig ConfigureStrong(this IAppConfig appConfig, Action<Config> configFunc)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (configFunc == null) throw new ArgumentNullException(nameof(configFunc));

            var config = appConfig.Strong();
            configFunc(config);
            return appConfig;
        }

        internal static IAppConfig EnsureInit(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.ConfigureStrong(config =>
            {
                if (config.Initialized)
                    return;

                config.Initialized = true;

                var lazyRegistry = new Lazy<GenericRegistry>(() =>
                {
                    var registry = new GenericRegistry();
                    config.RegistryConfigFunc.Value?.Invoke(registry);
                    return registry;
                });

                config.Registry.Value = () => lazyRegistry.Value;
            })
            .WrapAppRunner(runner => () =>
            {
                runner?.Invoke();
                var config = appConfig.Strong();
                JobManager.Initialize(config.Registry.Value());
            });
        }

        internal static Config Strong(this IAppConfig appConfig)
        {
            return new Config(appConfig);
        }
    }

    internal class GenericRegistry : Registry
    {
    }
}