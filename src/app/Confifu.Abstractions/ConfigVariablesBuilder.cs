using System;
using System.Collections.Generic;
using System.Linq;

namespace Confifu.Abstractions
{
    /// <summary>
    /// Helper class for combining different config variable sources
    /// </summary>
    public class ConfigVariablesBuilder : IConfigVariablesBuilder
    {
        protected List<IConfigVariablesBuilder> configParts = new List<IConfigVariablesBuilder>();

        public ConfigVariablesBuilder AddBuilder<T>(T builder) where T : IConfigVariablesBuilder
        {
            configParts.Add(builder);
            return this;
        }

        public ConfigVariablesBuilder Add(Func<IConfigVariables> factory)
        {
            return AddBuilder(new GenericConfigVariablesBulider(factory));
        }

        public ConfigVariablesBuilder Add(IConfigVariables configVariables)
        {
            return AddBuilder(new GenericConfigVariablesBulider(() => configVariables));
        }
        
        public IConfigVariables Build() => configParts.Any()
            ? (IConfigVariables)new ConfigVariablesChain(configParts.AsEnumerable().Reverse().Select(x => x.Build()))
            : new EmptyConfigVariables();
    }
}