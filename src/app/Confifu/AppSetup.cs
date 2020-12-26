using System;
using System.Collections.Generic;
using Confifu.Abstractions;

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
        public const string EnvKey = "AppEnv";

        private readonly AppConfig appConfig;
        
        private List<AppSetupAction> SetupActions { get; } = new List<AppSetupAction>();
        private string CurrentAppEnv => Env[EnvKey];

        /// <summary>
        /// Current IConfigVariables instance
        /// </summary>
        public IConfigVariables Env { get; }

        /// <summary>
        /// Current IAppConfig instance
        /// </summary>
        public IAppConfig AppConfig => appConfig;

        /// <summary>
        /// Creates new AppSetup instance with a given <para>env</para> ConfigVariables
        /// </summary>
        /// <param name="env">IConfigVariables instance</param>
        protected AppSetup(IConfigVariables env)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));

            Env = env;
            appConfig = new AppConfig();
            appConfig.SetConfigVariables(env);
        }

        /// <summary>
        /// Call configured SetupActions
        /// </summary>
        /// <returns>current AppSetup instance</returns>
        public AppSetup Setup()
        {
            foreach (var setupAction in SetupActions)
            {
                if (setupAction.AppEnv.IsIn(CurrentAppEnv))
                    setupAction.Action();
            }
            
            appConfig.GetPostSetupAction()?.Invoke();

            return this;
        }

        /// <summary>
        /// Call AppRunner from current IAppConfig
        /// </summary>
        /// <returns>current AppSetup instance</returns>
        public AppSetup Run()
        {
            appConfig.GetAppRunner()?.Invoke();
            appConfig.MarkSetupComplete();
            App.Config = appConfig;

            return this;
        }

        /// <summary>
        /// Add common setup action (will be always called during Setup phase)
        /// </summary>
        /// <param name="action">Action delegate</param>
        public void Configure(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            SetupActions.Add(new AppSetupAction(action, AppEnv.All));
        }

        /// <summary>
        /// Add an action for specific <para>environments</para> (will be called only 
        /// for given envronements during setup phase)
        /// </summary>
        /// <param name="action">Action delegate</param>
        /// <param name="appEnv">AppEnv instance</param>
        public void ConfigureFor(AppEnv appEnv, Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (appEnv == null) throw new ArgumentNullException(nameof(appEnv));

            SetupActions.Add(new AppSetupAction(action, appEnv));
        }
    }
}
