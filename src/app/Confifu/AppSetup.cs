using System;
using System.Collections.Generic;
using Confifu.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Confifu.Abstractions.DependencyInjection;

namespace Confifu
{
    /// <summary>
    /// Base Class for Application Config Setup.
    /// It'll create and help to configure IAppConfig, IServiceProvider, IServiceCollection
    /// 
    /// </summary>
    public class AppSetup
    {
        /// <summary>
        /// CSharpEnv predefined key
        /// </summary>
        public const string CSharpEnvKey = "CSharpEnv";

        private readonly AppConfig _appConfig;
        private readonly ServiceCollection _serviceCollection;
        
        private List<AppSetupAction> SetupActions { get; } = new List<AppSetupAction>();
        private string CSharpEnv => Env[CSharpEnvKey];

        /// <summary>
        /// Don't setup default service provider
        /// </summary>
        public bool AvoidDefaultServiceProvider { get; set; }

        /// <summary>
        /// Current IConfigVariables instance
        /// </summary>
        public IConfigVariables Env { get; }

        /// <summary>
        /// Current IAppConfig instance
        /// </summary>
        public IAppConfig AppConfig => _appConfig;

        /// <summary>
        /// Current IServiceCollection instance
        /// </summary>
        public IServiceCollection ServiceCollection => _serviceCollection;

        /// <summary>
        /// Creates new AppSetup instance with a given <para>env</para> ConfigVariables
        /// </summary>
        /// <param name="env">IConfigVariables instance</param>
        protected AppSetup(IConfigVariables env)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));

            Env = env;
            _appConfig = new AppConfig();
            _appConfig.SetConfigVariables(env);

            _serviceCollection = new ServiceCollection();
            _appConfig.SetServiceCollection(_serviceCollection);
            _appConfig.RegisterCommonServices();
        }

        /// <summary>
        /// Call configured SetupActions
        /// </summary>
        /// <returns>current AppSetup instance</returns>
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

            if (!AvoidDefaultServiceProvider && _appConfig.GetServiceProvider() == null)
                SetDefaultServiceProvider();
            
            return this;
        }

        private void SetDefaultServiceProvider()
        {
            _appConfig.SetServiceProvider(ServiceCollection.BuildServiceProvider());
        }

        /// <summary>
        /// Call AppRunner from curren IAppConfig
        /// </summary>
        /// <returns>current AppSetup instance</returns>
        public AppSetup Run()
        {
            _appConfig.GetAppRunner()?.Invoke();
            _appConfig.MarkSetupComplete();
            App.Config = _appConfig;

            return this;
        }

        /// <summary>
        /// Set ServiceProvider to current IAppConfig instance
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider instance</param>
        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            _appConfig.SetServiceProvider(serviceProvider);
        }

        /// <summary>
        /// Add common setup action (will be always called during Setup phase)
        /// </summary>
        /// <param name="action">Action delegate</param>
        public void Common(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            SetupActions.Add(new AppSetupAction(action));
        }

        /// <summary>
        /// Add an action for specific <para>environments</para> (will be called only 
        /// for given envronements during setup phase)
        /// </summary>
        /// <param name="action">Action delegate</param>
        /// <param name="environments">environments list</param>
        public void Environment(Action action, params string[] environments)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (environments == null) throw new ArgumentNullException(nameof(environments));

            foreach (var env in environments)
                Environment(env, action);
        }

        /// <summary>
        /// Add an action for specific <para>environment</para> (will be called only 
        /// for given envronement during setup phase)
        /// </summary>
        /// <param name="action">Action delegate</param>
        /// <param name="environment">environment</param>
        public void Environment(string environment, Action action)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            if (action == null) throw new ArgumentNullException(nameof(action));

            SetupActions.Add(new AppSetupAction(action, environment));
        }
    }
}
