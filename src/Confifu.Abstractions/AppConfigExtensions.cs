namespace Confifu.Abstractions
{
    public static class AppConfigExtensions
    {
        public const string ConfigVariablesKey = "ConfigVariables";
        public static T Get<T>(this IAppConfig appConfig, string key)
        {
            var config = appConfig[key];
            if (!(config is T))
                return default(T);
            return (T) config;
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