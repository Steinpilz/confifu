using ServiceStack.WebHost.Endpoints;

namespace ServiceStack.Confifu
{
    internal class Default
    {
        internal static void ConfigureAppHost(global::Confifu.Abstractions.IAppConfig appConfig,
            IAppHost host,
            EndpointHostConfig config)
        {
            // constainer is here
            var container = EndpointHost.Config.ServiceManager.Container;
            container.Adapter = new ContainerAdapter(appConfig);
        }
    }
}