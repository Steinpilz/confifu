using System;
using ServiceStack.WebHost.Endpoints;

namespace ServiceStack.Confifu
{
    public interface IRunnableAppHost : IAppHost
    {
        void Run(Action<EndpointHostConfig> configAction);
    }
}