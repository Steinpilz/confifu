namespace Confifu.Abstractions
{
    public static class AppConfigExtensions
    {
        public const string EnvironmentVariablesKey = "EnvironmentVariables";
        public static T Config<T>(this IAppConfig appConfig, string key)
        {
            var config = appConfig[key];
            if (!(config is T))
                return default(T);
            return (T) config;
        }

        public static IEnvironmentVariables GetEnvironmentVariables(this IAppConfig appConfig)
        {
            return appConfig.Config<IEnvironmentVariables>(EnvironmentVariablesKey);
        }

        public static void SetEnvironmentVariables(this IAppConfig appConfig,
            IEnvironmentVariables environmentVariables)
        {
            appConfig[EnvironmentVariablesKey] = environmentVariables;
        }
    }
}