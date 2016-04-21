using System;
using System.Collections.Generic;
using Confifu.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Confifu
{
    public class AppSetup
    {
        public const string CSharpEnvKey = "CSharpEnv";

        protected IConfigVariables Env { get; }

        private readonly AppConfig _appConfig;
        private readonly ServiceCollection _serviceCollection;

        protected IAppConfig AppConfig => _appConfig;

        private List<AppSetupAction> SetupActions { get; } = new List<AppSetupAction>();
        private string CSharpEnv => Env[CSharpEnvKey];

        protected AppSetup(IConfigVariables env)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));

            Env = env;
            _appConfig = new AppConfig();
            _appConfig.SetConfigVariables(env);

            _serviceCollection = new ServiceCollection();
            _appConfig.SetServiceCollection(_serviceCollection);
        }

        public void Setup()
        {
            var csharpEnv = CSharpEnv;
            foreach (var setupAction in SetupActions)
            {
                if (setupAction.Environment == null
                    || string.Compare(setupAction.Environment, csharpEnv,
                        StringComparison.CurrentCultureIgnoreCase) == 0)
                    setupAction.Action();
            }
            _appConfig.MarkSetupComplete();
            App.Config = _appConfig;
        }

        protected void LocateServices(Func<IServiceCollection, IServiceProvider> locateAction)
        {
            var serviceProvider = locateAction(_serviceCollection);
            _appConfig.SetServiceProvider(serviceProvider);
        }

        protected void Common(Action action)
        {
            SetupActions.Add(new AppSetupAction(action));
        }

        protected void Environment(string environment, Action action)
        {
            SetupActions.Add(new AppSetupAction(action, environment));
        }
    }
}
