using System;

namespace Confifu.Abstractions
{
    public static class ConfigVariablesExtensions
    {
        public static IConfigVariables WithPrefix(this IConfigVariables vars, string prefix)
        {
            if (vars == null) throw new ArgumentNullException(nameof(vars));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            return new ConfigVariablesWrapper(vars, prefix);
        }
    }
}