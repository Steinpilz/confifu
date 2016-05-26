using System;
using System.Linq.Expressions;

namespace Confifu.Abstractions
{
    /// <summary>
    /// Base class which helps to organize Strong-typed library configuration classes
    /// </summary>
    public class LibraryConfig
    {
        /// <summary>
        /// Prefix given to a constructor
        /// </summary>
        protected string Prefix { get; }

        /// <summary>
        /// IAppConfig given to a constructor
        /// </summary>
        protected IAppConfig RootAppConfig { get; }

        /// <summary>
        /// IConfigVariables of RootAppConfig
        /// </summary>
        protected IConfigVariables RootVars { get; }

        /// <summary>
        /// Prefixed IAppConfig instance of given RootAppConfig
        /// </summary>
        protected IAppConfig AppConfig { get; }

        /// <summary>
        /// Prefixed IConfigVariables instance of given RootVars
        /// </summary>
        protected IConfigVariables Vars { get; }

        /// <summary>
        /// Creates new instance with given <para>appConfig</para> and <para>prefix</para>
        /// </summary>
        /// <param name="appConfig">IAppConfig instance</param>
        /// <param name="prefix">prefix string</param>
        public LibraryConfig(IAppConfig appConfig, string prefix)
        {
            if (appConfig == null) throw new ArgumentNullException(nameof(appConfig));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            RootAppConfig = appConfig;
            RootVars = appConfig.GetConfigVariables();
            Prefix = prefix;
            
            AppConfig = new AppConfigWrapper(RootAppConfig, Prefix);
            Vars = new ConfigVariablesWrapper(RootVars, Prefix);
        }
        
        /// <summary>
        /// This is going to be a marker is the library initialized 
        /// </summary>
        public bool Initialized
        {
            get { return AppConfig.Get<bool>("Initialized"); }
            set { AppConfig["Initialized"] = value; }
        }
        
        /// <summary>
        /// Returns ConfigProperty instance for AppConfig using given <para>propertySelector</para>
        /// to determine PropertyKey
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="propertySelector">Property Selector expression to determine property key. 
        /// <code>() => MyCustomProperty</code> "MyCustomProperty" would be the key</param>
        /// <returns></returns>
        protected ConfigProperty<T> Property<T>(Expression<Func<ConfigProperty<T>>> propertySelector) where T:class
        {
            if (propertySelector == null) throw new ArgumentNullException(nameof(propertySelector));

            return Property<T>(propertySelector.GetPropertyName());
        }

        /// <summary>
        /// Returns ConfigProperty instance for AppConfig using given <para>key</para>
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="key">key string</param>
        /// <returns>ConfigProperty</returns>
        protected ConfigProperty<T> Property<T>(string key) where T : class
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return new ConfigProperty<T>(AppConfig, key);
        }
    }
}