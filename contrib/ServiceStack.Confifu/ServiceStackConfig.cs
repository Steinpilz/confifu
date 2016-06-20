using System;
using System.Collections.Generic;
using System.Reflection;
using ServiceStack.WebHost.Endpoints;

namespace ServiceStack.Confifu
{
    /// <summary>
    /// ServiceStack config
    /// </summary>
    public class ServiceStackConfig
    {
        /// <summary>
        /// AppHost factory
        /// </summary>
        public Func<IRunnableAppHost> AppHost { get; set; }

        /// <summary>
        /// EndpointHostConfig configuration action
        /// </summary>
        public Action<IAppHost, EndpointHostConfig> Config { get; set; }

        /// <summary>
        /// Default ServiceHost Name
        /// </summary>
        public string ServiceHostName { get; set; }

        /// <summary>
        /// Assemblies with services. Used by default
        /// </summary>
        public List<Assembly> ServiceHostAssemblies { get; } = new List<Assembly>();

        /// <summary>
        /// Added configuration action
        /// </summary>
        /// <param name="configAction">configuration action delegate</param>
        public void AddConfigAction(Action<IAppHost, EndpointHostConfig> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            var current = Config;
            if (current == null)
                Config = configAction;
            else
                Config = (appHost, config) =>
                {
                    current(appHost, config);
                    configAction(appHost, config);
                };
        }
    }
}