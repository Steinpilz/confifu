using System;
using Confifu.Abstractions;
using ServiceStack.Configuration;
using Confifu.Abstractions.DependencyInjection;

namespace ServiceStack.Confifu
{
    /// <summary>
    /// ServiceStack Container adapter for Confifu.DependencyIncjetion
    /// </summary>
    public class ContainerAdapter : IContainerAdapter
    {
        private readonly IAppConfig _appConfig;

        /// <summary>
        /// Create new ContainerAdapter instance based on given <para>appConfig</para>
        /// </summary>
        /// <param name="appConfig"></param>
        public ContainerAdapter(IAppConfig appConfig)
        {
            _appConfig = appConfig;
        }

        private IServiceProvider ServiceProvider()
        {
            return _appConfig.GetServiceProvider();
        }

        public T TryResolve<T>()
        {
            return (T)ServiceProvider().GetService(typeof (T));
        }

        public T Resolve<T>()
        {
            return TryResolve<T>();
        }
    }
}