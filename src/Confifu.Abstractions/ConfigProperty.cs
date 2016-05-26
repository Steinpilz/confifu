using System;

namespace Confifu.Abstractions
{
    /// <summary>
    /// Class for accessing single property by key in given IAppConfig instance
    /// </summary>
    /// <typeparam name="T">Property Type</typeparam>
    public class ConfigProperty<T> where T : class
    {
        private readonly IAppConfig _appConfig;
        private readonly string _key;

        /// <summary>
        /// Creates new ConfigProperty instance with <para>appConfig</para> and <para>key</para>
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="key">Property Key string</param>
        public ConfigProperty(IAppConfig appConfig, string key)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (key == null) throw new ArgumentNullException(nameof(key));

            _appConfig = appConfig;
            _key = key;
        }

        /// <summary>
        /// Returns Property Value casted to T
        /// </summary>
        public T Value
        {
            get { return Get(); }
            set { Set(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Property Value casted to T</returns>
        public T Get()
        {
            return _appConfig[_key] as T;
        }

        /// <summary>
        /// Sets given Property Value
        /// </summary>
        /// <param name="value">Property Value</param>
        public void Set(T value)
        {
            _appConfig[_key] = value;
        }
        
        /// <summary>
        /// Operator to convert ConfigProperty to Property Value T
        /// </summary>
        /// <param name="property">ConfigProperty instance</param>
        public static implicit operator T(ConfigProperty<T> property)
        {
            return property.Get();
        }
    }
}