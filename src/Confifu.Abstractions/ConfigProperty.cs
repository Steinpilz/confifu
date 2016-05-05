using System;

namespace Confifu.Abstractions
{
    public class ConfigProperty<T> where T : class
    {
        private readonly IAppConfig _appConfig;
        private readonly string _key;

        public ConfigProperty(IAppConfig appConfig, string key)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (key == null) throw new ArgumentNullException(nameof(key));

            _appConfig = appConfig;
            _key = key;
        }

        public T Value
        {
            get { return Get(); }
            set { Set(value); }
        }

        public T Get()
        {
            return _appConfig[_key] as T;
        }

        public void Set(T value)
        {
            _appConfig[_key] = value;
        }
        
        public static implicit operator T(ConfigProperty<T> property)
        {
            return property.Get();
        }
    }
}