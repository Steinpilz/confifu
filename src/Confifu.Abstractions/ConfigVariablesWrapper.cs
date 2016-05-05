using System;

namespace Confifu.Abstractions
{
    public class ConfigVariablesWrapper : IConfigVariables
    {
        private readonly IConfigVariables _configVariables;
        private readonly string _prefix;

        public string this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                return _configVariables[_prefix + key];
            }
        }

        public ConfigVariablesWrapper(IConfigVariables configVariables, string prefix = "")
        {
            if(configVariables == null)
                throw new ArgumentNullException(nameof(configVariables));

            if(prefix == null)
                throw new ArgumentNullException(nameof(prefix));

            _configVariables = configVariables;
            _prefix = prefix;
        }

        public ConfigVariablesWrapper AddPrefix(string prefix)
        {
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            return new ConfigVariablesWrapper(this, _prefix + prefix);
        }
    }
}