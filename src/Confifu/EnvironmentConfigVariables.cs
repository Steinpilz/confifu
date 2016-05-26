using System;
using Confifu.Abstractions;

namespace Confifu
{
    /// <summary>
    /// IConfigVariables implementation.
    /// Variables is given from Environment Variables
    /// </summary>
    public class EnvironmentConfigVariables : IConfigVariables
    {
        /// <summary>
        /// Returns Environment Variable value with a given <para>key</para>
        /// </summary>
        /// <param name="key">key string</param>
        /// <returns>variable</returns>
        public string this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                return Environment.GetEnvironmentVariable(key);
            }
        }
    }
}