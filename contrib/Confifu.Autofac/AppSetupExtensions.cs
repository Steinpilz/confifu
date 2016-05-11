using System;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;

namespace Confifu.Autofac
{
    public static class AppSetupExtensions
    {
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