using System;

namespace Confifu.Abstractions
{

    public class GenericConfigVariablesBulider : IConfigVariablesBuilder
    {
        private readonly Func<IConfigVariables> factory;

        public GenericConfigVariablesBulider(Func<IConfigVariables> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            this.factory = factory;
        }

        public IConfigVariables Build() => factory();
    }
}