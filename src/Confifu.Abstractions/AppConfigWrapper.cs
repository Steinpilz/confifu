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
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                return _appConfig[_prefix + key];
            }
            set
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                _appConfig[_prefix+key] = value;
            }
        }
    }
}