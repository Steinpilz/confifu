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

        public T AddBuilder<T>(T builder) where T : IConfigVariablesBuilder
        {
            configParts.Add(builder);
            return builder;
        }

        public IConfigVariables Build() => configParts.Any()
            ? (IConfigVariables)new ConfigVariablesChain(configParts.AsEnumerable().Reverse().Select(x => x.Build()))
            : new EmptyConfigVariables();
    }
}