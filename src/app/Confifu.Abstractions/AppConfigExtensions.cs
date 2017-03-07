using System;

namespace Confifu.Abstractions
{
    /// <summary>
    /// Class holding IAppConfig extension methods
    /// </summary>
    public static class AppConfigExtensions
    {
        /// <summary>
        /// ConfigVariables predefined key
        /// </summary>
        public const string ConfigVariablesKey = "ConfigVariables";

        /// <summary>
        /// AppRunner predefined key
        /// </summary>
        public const string AppRunnerKey = "AppRunner";

        /// <summary>
        /// Get option from <para>appConfig</para>
        /// </summary>
        /// <typeparam name="T">option type</typeparam>
        /// <param name="appConfig">AppConfig instance</param>
        /// <param name="key">option key</param>
        /// <returns>option casted to <typeparam>T</typeparam></returns>
        public static T Get<T>(this IAppConfig appConfig, string key)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var config = appConfig[key];
            if (!(config is T))
                return default(T);
            return (T) config;
        }
        
        /// <summary>
        /// Get Config Variables from <para>appConfig</para> using ConfigVariables predefined key
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <returns>IConfigVariables instance</returns>
        public static IConfigVariables GetConfigVariables(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return appConfig.Get<IConfigVariables>(ConfigVariablesKey);
        }

        /// <summary>
        /// Set Config Variables to <para>appConfig</para> using ConfigVariables predefined key
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <param name="configVariables">IConfigVariables instance</param>
        public static void SetConfigVariables(this IAppConfig appConfig,
            IConfigVariables configVariables)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (configVariables == null) throw new ArgumentNullException(nameof(configVariables));

            appConfig[ConfigVariablesKey] = configVariables;
        }

        /// <summary>
        /// Get AppRunner from <para>appConfig</para> using AppRunner predefined key
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <returns>delegate of Action type, always not null</returns>
        public static Action GetAppRunner(this IAppConfig appConfig)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));

            return (appConfig.Get<Action>(AppRunnerKey)) ?? (() => { });
        }

        /// <summary>
        /// Set AppRunner to <para>appConfig</para> using AppRunner predefined key
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <param name="appRunner">AppRunner delegate</param>
        public static void SetAppRunner(this IAppConfig appConfig, Action appRunner)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (appRunner == null) throw new ArgumentNullException(nameof(appRunner));

            appConfig[AppRunnerKey] = appRunner;
        }

        /// <summary>
        /// Set return by <para>wrapFunc</para> AppRunner to <para>appConfig</para> using AppRunner predefined key
        /// <para>wrapFunc</para> has single parameter - current AppRunner (could be null)
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <param name="wrapFunc">WrapFunc delegate</param>
        /// <returns>AppConfig instance passed to the method</returns>
        public static IAppConfig WrapAppRunner(this IAppConfig appConfig, Func<Action, Action> wrapFunc)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (wrapFunc == null) throw new ArgumentNullException(nameof(wrapFunc));

            appConfig.SetAppRunner(wrapFunc(appConfig.GetAppRunner()));
            return appConfig;
        }

        /// <summary>
        /// Set AppRunner action to be executed after current AppRunner action
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <param name="appRunner">AppRunner delegate</param>
        /// <returns>AppConfig instance passed to the method</returns>
        public static IAppConfig AddAppRunnerAfter(this IAppConfig appConfig, Action appRunner)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (appRunner == null) throw new ArgumentNullException(nameof(appRunner));

            return appConfig.WrapAppRunner(runner => () =>
            {
                // call existing runner, and then ours
                runner?.Invoke();
                appRunner();
            });
        }

        /// <summary>
        /// Set AppRunner action to be executed after current AppRunner action
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <param name="appRunner">AppRunner delegate</param>
        /// <returns>AppConfig instance passed to the method</returns>
        public static IAppConfig AddRunner(this IAppConfig appConfig, Action appRunner)
        {
            return appConfig.AddAppRunnerAfter(appRunner);
        }

        /// <summary>
        /// Set AppRunner action to be executed before existing AppRunner action
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <param name="appRunner">AppRunner delegate</param>
        /// <returns>AppConfig instance passed to the method</returns>
        public static IAppConfig AddAppRunnerBefore(this IAppConfig appConfig, Action appRunner)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (appRunner == null) throw new ArgumentNullException(nameof(appRunner));
            return appConfig.WrapAppRunner(runner => () =>
            {
                // call our runner, and then existing
                appRunner();
                runner?.Invoke();
            });
        }

        /// <summary>
        /// Returns new IAppConfig instance, which will prefix all option accessors with
        /// a given <para>prefix</para>
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <param name="prefix">Prefix string</param>
        /// <returns>new IAppConfig instance</returns>
        public static IAppConfig WithPrefix(this IAppConfig appConfig, string prefix)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            return new AppConfigWrapper(appConfig, prefix);
        }

        /// <summary>
        /// Sets to IAppConfig instance ConfigVariables and returns the given instance
        /// </summary>
        /// <param name="appConfig">AppConfig instance</param>
        /// <param name="configVariables">ConfigVariables instance</param>
        /// <returns>Given IAppConfig instance</returns>
        public static IAppConfig WithConfigVariables(this IAppConfig appConfig, IConfigVariables configVariables)
        {
            appConfig.SetConfigVariables(configVariables);
            return appConfig;
        }

        /// <summary>
        /// Ensures that <paramref name="initializerAction"/> runs once (scoped by <paramref name="module"/> in given <paramref name="appConfig"/> instance)
        /// </summary>
        /// <param name="appConfig"></param>
        /// <param name="module"></param>
        /// <param name="initializerAction"></param>
        /// <returns></returns>
        public static IAppConfig RunOnce(this IAppConfig appConfig, string module, Action initializerAction)
        {
            var key = $"{module}:__Initialized";
            if ( (appConfig[key] as bool?) == true)
                return appConfig;

            initializerAction?.Invoke();

            appConfig[key] = true;

            return appConfig;
        }

        /// <summary>
        /// Creates singleton config in <paramref name="module"/> and runs once <paramref name="oneTimeAction"/>
        /// Returns created or existing config
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="appConfig"></param>
        /// <param name="module"></param>
        /// <param name="configFunc"></param>
        /// <param name="oneTimeAction"></param>
        /// <returns></returns>
        public static TConfig EnsureConfig<TConfig>(this IAppConfig appConfig, 
            string module,
            Func<TConfig> configFunc, 
            Action<TConfig> oneTimeAction)
        {
            var config = appConfig[module + ":__Config"];
            if(config == null)
            {
                config = configFunc();
                appConfig[module + ":__Config"] = config;

                oneTimeAction((TConfig)config);
            }

            return (TConfig)config;
        }


        /// <summary>
        /// Creates singleton config in <paramref name="module"/> and runs once <paramref name="oneTimeAction"/>
        /// Returns created or existing config
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="appConfig"></param>
        /// <param name="module"></param>
        /// <param name="oneTimeAction"></param>
        /// <returns></returns>
        public static TConfig EnsureConfig<TConfig>(this IAppConfig appConfig, string module, Action<TConfig> oneTimeAction)
            where TConfig : new()
        {
            return appConfig.EnsureConfig(module, () => new TConfig(), oneTimeAction);
        }

    }
}