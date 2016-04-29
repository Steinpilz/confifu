using System;

namespace Confifu.Abstractions
{
    public class AppConfigWrapper : IAppConfig
    {
        private readonly IAppConfig _appConfig;
        private readonly string _prefix;

        public AppConfigWrapper(IAppConfig appConfig, string prefix = "")
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            _appConfig = appConfig;
            _prefix = prefix;
        }

        public object this[string key]
        {
            get { return _appConfig[_prefix + key]; }
            set { _appConfig[_prefix+key] = value;}
        }
    }
}