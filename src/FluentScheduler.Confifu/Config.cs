using System;
using Confifu.Abstractions;

namespace FluentScheduler.Confifu
{
    public static class Const
    {
        public const string Registry = "Registry";
        public const string RegistryConfigFunc = "RegistryConfigFunc";
        public const string Initialized = "Initialized";
    }

    public class Config
    {
        private readonly ConfigVariablesWrapper _vars;
        private readonly IAppConfig _appConfig;
        public static Config Current => new Config(App.Config);

        public Config(IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            _appConfig = appConfig;
            _vars = new ConfigVariablesWrapper(appConfig.GetConfigVariables(), "FluentScheduler:");
        }

        public bool Initialized
        {
            get { return _appConfig.Get<bool>(Const.Initialized); }
            set { _appConfig[Const.Initialized] = value; }
        }

        public Func<Registry> Registry
        {
            get { return _appConfig[Const.Registry] as Func<Registry>; } 
            set { _appConfig[Const.Registry] = value; }
        }

        public Action<Registry> RegistryConfigFunc
        {
            get { return _appConfig[Const.RegistryConfigFunc] as Action<Registry>; } 
            set { _appConfig[Const.RegistryConfigFunc] = value; }
        }
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
                    config.RegistryConfigFunc?.Invoke(registry);
                    return registry;
                });

                config.Registry = () => lazyRegistry.Value;
            })
            .WrapAppRunner(runner => () =>
            {
                runner?.Invoke();
                var config = appConfig.Strong();
                JobManager.Initialize(config.Registry());
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
                var currentConfigFunc = config.RegistryConfigFunc;
                config.RegistryConfigFunc = registry =>
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