using System;
using Confifu.Abstractions;

namespace FluentScheduler.Confifu
{
    public static class Const
    {
    }

    public class Config : LibraryConfig
    {
        public static Config Current => new Config(App.Config);

        public Config(IAppConfig appConfig) : base(appConfig,  "FluentScheduler:")
        {
        }

        public ConfigProperty<Func<Registry>> Registry => Property(() => Registry);
        public ConfigProperty<Action<Registry>> RegistryConfigFunc => Property(() => RegistryConfigFunc);
    }

    public static class AppConfigExtensions
    {
        public static IAppConfig InitFluentScheduler(this IAppConfig appConfig)
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

        public static IAppConfig UseFluentScheduler(this IAppConfig appConfig, 
            Action<Registry> configureFunc)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (configureFunc == null) throw new ArgumentNullException(nameof(configureFunc));

            return appConfig
                .InitFluentScheduler()
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

        public static IAppConfig ConfigureStrong(this IAppConfig appConfig, Action<Config> configFunc)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (configFunc == null) throw new ArgumentNullException(nameof(configFunc));

            var config = appConfig.Strong();
            configFunc(config);
            return appConfig;
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