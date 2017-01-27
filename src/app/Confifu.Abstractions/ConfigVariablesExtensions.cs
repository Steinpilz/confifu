using System;

namespace Confifu.Abstractions
{
    /// <summary>
    /// Class holing IConfigVariables extension methods
    /// </summary>
    public static class ConfigVariablesExtensions
    {
        /// <summary>
        /// Returns new IConfigVariables instance which is prefixing all variable accessors
        /// with a given <para>prefix</para>
        /// </summary>
        /// <param name="vars">IConfigVariables instance</param>
        /// <param name="prefix">prefix string</param>
        /// <returns>new IConfigVariables instance</returns>
        public static IConfigVariables WithPrefix(this IConfigVariables vars, string prefix)
        {
            if (vars == null) throw new ArgumentNullException(nameof(vars));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            return new ConfigVariablesWrapper(vars, prefix);
        }
    }
}