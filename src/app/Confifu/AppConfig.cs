using System;
using System.Collections.Generic;
using Confifu.Abstractions;

namespace Confifu
{
    /// <summary>
    /// IAppConfig implementation based on Dictionary to store config options.
    /// It's supposed to be configured at application start up in a single thread
    /// and after setup is complete it should be readonly since get/set operations
    /// aren't thread safe together.
    /// </summary>
    public class AppConfig : IAppConfig
    {
        private volatile bool _isSetupComplete;
        private readonly IDictionary<string, object> _dictionary = new Dictionary<string, object>();

        /// <summary>
        /// Marks that Setup phase is complete
        /// All future modifications will throw exception.
        /// </summary>
        public void MarkSetupComplete()
        {
            _isSetupComplete = true;
        }

        /// <summary>
        /// Get/Set config option
        /// </summary>
        /// <param name="key">key string (not null)</param>
        /// <returns>config option</returns>
        public object this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                return GetValue(key);
            }
            set
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                SetValue(key, value);
            }
        }

        private object GetValue(string key)
        {
            return SafeGet(key);
        }

        private void SetValue(string key, object value)
        {
            if (_isSetupComplete)
                throw new InvalidOperationException("AppConfig cannot be modified after it's marked as Setup Complete");
            _dictionary[key] = value;
        }

        private object SafeGet(string key)
        {
            object returnValue;
            return _dictionary.TryGetValue(key, out returnValue) ? returnValue : null;
        }
    }
}