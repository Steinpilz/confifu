using System;

namespace Confifu
{
    class AppSetupAction
    {
        public AppSetupAction(Action action, string environment = null)
        {
            Action = action;
            Environment = environment;
        }

        public string Environment { get; }
        public Action Action { get; }
    }
}