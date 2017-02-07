using System;

namespace Confifu
{
    internal class AppSetupAction
    {
        public AppSetupAction(Action action, AppEnv appEnv)
        {
            Action = action;
            AppEnv = appEnv;
        }

        public AppEnv AppEnv { get; }
        public Action Action { get; }
    }
}