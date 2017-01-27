using System;

namespace Confifu.Abstractions
{
    /// <summary>
    /// This class is supposed to hold single IAppConfig instance in the Application
    /// </summary>
    public static class App
    {
        /// <summary>
        /// Singleton IAppConfig instance. Should be set prior to usage. Throws exception if not set.
        /// </summary>
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

        /// <summary>
        /// Config Variables of current AppConfig
        /// </summary>
        public static IConfigVariables Vars => Config.GetConfigVariables();

        /// <summary>
        /// Get item from current Config
        /// </summary>
        /// <typeparam name="T">type of config option</typeparam>
        /// <param name="key">key of config option</param>
        /// <returns>config option casted to type <typeparam>T</typeparam></returns>
        public static T GetConfig<T>(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return Config.Get<T>(key);
        }
    }
}