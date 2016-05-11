using System;
using System.Reflection;
using Funq;
using ServiceStack.WebHost.Endpoints;

namespace ServiceStack.Confifu
{
    public class AppHostWebListener : AppHostBase, IRunnableAppHost
    {
        private Action<EndpointHostConfig> _configAction;

        public AppHostWebListener(string serviceName, 
            params Assembly[] assembliesWithServices) 
            : base(serviceName, assembliesWithServices)
        {
        }

        public override void Configure(Container container)
        {
            var config = new EndpointHostConfig();

            _configAction?.Invoke(config);

            SetConfig(config);
        }

        public void Run(Action<EndpointHostConfig> configAction)
        {
            _configAction = configAction;
            Init();
        }
    }
}