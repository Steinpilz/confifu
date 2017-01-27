using System;
using System.Reflection;
using Funq;
using ServiceStack.WebHost.Endpoints;

namespace ServiceStack.Confifu
{
    /// <summary>
    /// AppHostHttpListenreBase wrapper implementing IRunnableAppHost
    /// </summary>
    public class AppHostHttpListener : AppHostHttpListenerBase, IRunnableAppHost
    {
        private Action<EndpointHostConfig> _configAction;
        private readonly string _url;

        public AppHostHttpListener(string url)
        {
            _url = url;
        }

        public AppHostHttpListener(string serviceName,
            string url, params Assembly[] assembliesWithServices) : base(serviceName, assembliesWithServices)
        {
            _url = url;
        }

        public AppHostHttpListener(string serviceName, 
            string handlerPath, string url, params Assembly[] assembliesWithServices) 
            : base(serviceName, handlerPath, assembliesWithServices)
        {
            _url = url;
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
            Start(_url);
        }
    }
}