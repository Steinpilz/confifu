using System;
using System.Linq.Expressions;

namespace Confifu.Abstractions
{
    public class LibraryConfig
    {
        protected string Prefix { get; }
        protected IAppConfig RootAppConfig { get; }
        protected IConfigVariables RootVars { get; }
        protected IAppConfig AppConfig { get; }
        protected IConfigVariables Vars { get; }

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
        
        public bool Initialized
        {
            get { return AppConfig.Get<bool>("Initialized"); }
            set { AppConfig["Initialized"] = value; }
        }

        protected ConfigProperty<T> Property<T>(Expression<Func<ConfigProperty<T>>> propertySelector) where T:class
        {
            if (propertySelector == null) throw new ArgumentNullException(nameof(propertySelector));

            return Property<T>(propertySelector.GetPropertyName());
        }

        protected ConfigProperty<T> Property<T>(string key) where T : class
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return new ConfigProperty<T>(AppConfig, key);
        }
    }
}