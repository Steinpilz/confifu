using System;

namespace Confifu.Abstractions
{
    public static class App
    {
        public static IAppConfig Config
        {
            get
            {
                if (_config == null)
                    throw new InvalidOperationException("App.Config should be set prior to its usage");
                return _config;
            }
            set { _config = value; }
        }

        private static IAppConfig _config;

        public static ConfigVariablesWrapper Vars => new ConfigVariablesWrapper(Config.GetConfigVariables());

        public static T GetConfig<T>(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return Config.Get<T>(key);
        }
    }
}