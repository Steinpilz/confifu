using System;

namespace Confifu.Abstractions
{
    /// <summary>
    /// IConfigVariables Wrapper providing Prefixing functionality:
    /// All Variables accessors are redirected to a given IConfigVariables 
    /// with a given prefix
    /// </summary>
    public class ConfigVariablesWrapper : IConfigVariables
    {
        private readonly IConfigVariables _configVariables;
        private readonly string _prefix;

        /// <summary>
        /// Returns Variable using given prefix+key as a new key
        /// </summary>
        /// <param name="key">key string</param>
        /// <returns>variables</returns>
        public string this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                return _configVariables[_prefix + key];
            }
        }

        /// <summary>
        /// Creates new ConfigVariablesWrapper instance for underlying <para>configVariables</para> with 
        /// given <para>prefix</para>
        /// </summary>
        /// <param name="configVariables">configVariables instance</param>
        /// <param name="prefix">prefix string</param>
        public ConfigVariablesWrapper(IConfigVariables configVariables, string prefix = "")
        {
            if(configVariables == null)
                throw new ArgumentNullException(nameof(configVariables));

            if(prefix == null)
                throw new ArgumentNullException(nameof(prefix));

            _configVariables = configVariables;
            _prefix = prefix;
        }
    }
}