using System;
using ServiceStack.WebHost.Endpoints;

namespace ServiceStack.Confifu
{
    /// <summary>
    /// Interface helping to provide Confifu configuration
    /// to given IAppHost
    /// </summary>
    public interface IRunnableAppHost : IAppHost
    {
        void Run(Action<EndpointHostConfig> configAction);
    }
}