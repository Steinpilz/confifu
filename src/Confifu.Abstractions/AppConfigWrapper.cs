using System;

namespace Confifu.Abstractions
{
    /// <summary>
    /// IAppConfig Wrapper providing Prefixing functionality:
    /// All Options accessors are redirected to a given IAppConfig 
    /// with a given prefix
    /// </summary>
    public class AppConfigWrapper : IAppConfig
    {
        private readonly IAppConfig _appConfig;
        private readonly string _prefix;

        /// <summary>
        /// Create new AppConfigWrapper instance, wrapping given <para>appConfig</para> with given <para>prefix</para>
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="prefix"></param>
        public AppConfigWrapper(IAppConfig appConfig, string prefix = "")
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            _appConfig = appConfig;
            _prefix = prefix;
        }

        /// <summary>
        /// Returns Config Option of underlying appConfig using [prefix + key] as a new key
        /// </summary>
        /// <param name="key">Config Option key</param>
        /// <returns>Config Option</returns>
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