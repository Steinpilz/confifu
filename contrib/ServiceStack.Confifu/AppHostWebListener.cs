using System;
using System.Reflection;
using Funq;
using ServiceStack.WebHost.Endpoints;

namespace ServiceStack.Confifu
{
    /// <summary>
    /// AppHostBase wrapper implementing IRunnableAppHost
    /// </summary>
    public class AppHostWebListener : AppHostBase, IRunnableAppHost
    {
        private Action<EndpointHostConfig> _configAction;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="assembliesWithServices"></param>
        public AppHostWebListener(string serviceName, 
            params Assembly[] assembliesWithServices) 
            : base(serviceName, assembliesWithServices)
        {
        }

        /// <summary>
        /// Configure Host
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            var config = new EndpointHostConfig();

            _configAction?.Invoke(config);

            SetConfig(config);
        }

        /// <summary>
        /// Run current AppHost
        /// </summary>
        /// <param name="configAction"></param>
        public void Run(Action<EndpointHostConfig> configAction)
        {
            _configAction = configAction;
            Init();
        }
    }
}