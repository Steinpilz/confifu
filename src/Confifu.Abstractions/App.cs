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
                    throw new InvalidOperationException("Env.Config should be set prior to its usage");
                return _config;
            }
            set { _config = value; }
        }

        private static IAppConfig _config;

        public static IEnvironmentVariables EnvVars => Config.GetEnvironmentVariables();
    }
}