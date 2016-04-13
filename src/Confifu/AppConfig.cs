using System;
using System.Collections.Generic;
using Confifu.Abstractions;

namespace Confifu
{
    public class AppConfig : IAppConfig
    {
        private bool _isSetupComplete;
        private readonly IDictionary<string, object> _dictionary = new Dictionary<string, object>();

        public void MarkSetupComplete()
        {
            _isSetupComplete = true;
        }

        public object this[string key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        private object GetValue(string key)
        {
            return SafeGet(key);
        }

        private void SetValue(string key, object value)
        {
            if (_isSetupComplete)
                throw new InvalidOperationException("AppConfig cannot be modified after it's marked as Setufp Complete");
            _dictionary[key] = value;
        }

        private object SafeGet(string key)
        {
            object returnValue;
            if (_dictionary.TryGetValue(key, out returnValue))
                return returnValue;
            return null;
        }
    }
}