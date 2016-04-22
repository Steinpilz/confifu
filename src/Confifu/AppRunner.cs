using System;
using Confifu.Abstractions;

namespace Confifu
{
    public class AppRunner : IAppRunner
    {
        private readonly Action _action;

        public AppRunner(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            _action = action;
        }

        public void Run()
        {
            _action();
        }
    }
}