using System;
using System.Collections.Generic;
using Confifu.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Confifu.Abstractions.DependencyInjection;

namespace Confifu
{
    public class AppSetup
    {
        public const string CSharpEnvKey = "CSharpEnv";

        protected IConfigVariables Env { get; }

        private readonly AppConfig _appConfig;
        private readonly ServiceCollection _serviceCollection;

        protected IAppConfig AppConfig => _appConfig;
        protected IServiceCollection ServiceCollection => _serviceCollection;

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

        public AppSetup Setup()
        {
            var csharpEnv = CSharpEnv;
            foreach (var setupAction in SetupActions)
            {
                if (setupAction.Environment == null
                    || string.Compare(setupAction.Environment, csharpEnv,
                        StringComparison.CurrentCultureIgnoreCase) == 0)
                    setupAction.Action();
            }
            App.Config = _appConfig;
            return this;
        }

        public AppSetup Run()
        {
            _appConfig.GetAppRunner()?.Invoke();
            _appConfig.MarkSetupComplete();
            return this;
        }

        protected void SetServiceProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            _appConfig.SetServiceProvider(serviceProvider);
        }

        protected void Common(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            SetupActions.Add(new AppSetupAction(action));
        }

        protected void Environment(Action action, params string[] environments)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (environments == null) throw new ArgumentNullException(nameof(environments));

            foreach (var env in environments)
                Environment(env, action);
        }

        protected void Environment(string environment, Action action)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            if (action == null) throw new ArgumentNullException(nameof(action));

            SetupActions.Add(new AppSetupAction(action, environment));
        }
    }
}
