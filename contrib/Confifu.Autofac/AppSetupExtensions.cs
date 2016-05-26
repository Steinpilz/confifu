using System;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;

namespace Confifu.Autofac
{
    /// <summary>
    /// Class holding AppSetup extensions to help setup Autofac container
    /// </summary>
    public static class AppSetupExtensions
    {
        /// <summary>
        /// Use Autofac container as IServiceProvider in given <para>appSetup</para> phase.
        /// </summary>
        /// <param name="appSetup">AppSetup instance</param>
        /// <param name="configAction">Autofac Containerbuilder custom configAction called before 
        /// building the container</param>
        /// <returns>built IContainer instance</returns>
        public static IContainer SetupAutofacContainer(this AppSetup appSetup, Action<ContainerBuilder> configAction = null)
        {
            if (appSetup == null) throw new ArgumentNullException(nameof(appSetup));

            var builder = new ContainerBuilder();
            builder.Populate(appSetup.ServiceCollection);

            configAction?.Invoke(builder);

            var container = builder.Build();
            appSetup.SetServiceProvider(container.Resolve<IServiceProvider>());

            return container;
        }
    }
}