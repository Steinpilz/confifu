using System;
using ServiceStack.WebHost.Endpoints;

namespace ServiceStack.Confifu
{
    public class ServiceStackConfig
    {
        public Func<IRunnableAppHost> AppHost { get; set; }
        public Action<IAppHost, EndpointHostConfig> Config { get; set; }

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