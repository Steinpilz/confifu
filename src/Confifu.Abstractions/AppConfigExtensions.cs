using System;

namespace Confifu.Abstractions
{
    public static class AppConfigExtensions
    {
        public const string ConfigVariablesKey = "ConfigVariables";
        public const string AppRunnerKey = "AppRunner";

        public static T Get<T>(this IAppConfig appConfig, string key)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var config = appConfig[key];
            if (!(config is T))
                return default(T);
            return (T) config;
        }
        
        public static IConfigVariables GetConfigVariables(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.Get<IConfigVariables>(ConfigVariablesKey);
        }

        public static void SetConfigVariables(this IAppConfig appConfig,
            IConfigVariables configVariables)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (configVariables == null) throw new ArgumentNullException(nameof(configVariables));

            appConfig[ConfigVariablesKey] = configVariables;
        }

        public static Action GetAppRunner(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.Get<Action>(AppRunnerKey);
        }

        public static void SetAppRunner(this IAppConfig appConfig, Action appRunner)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (appRunner == null) throw new ArgumentNullException(nameof(appRunner));

            appConfig[AppRunnerKey] = appRunner;
        }

        public static IAppConfig WrapAppRunner(this IAppConfig appConfig, Func<Action, Action> wrapFunc)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (wrapFunc == null) throw new ArgumentNullException(nameof(wrapFunc));

            appConfig.SetAppRunner(wrapFunc(appConfig.GetAppRunner()));
            return appConfig;
        }
    }
}