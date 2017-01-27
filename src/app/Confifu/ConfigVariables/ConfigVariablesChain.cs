using System;
using System.Collections.Generic;
using System.Linq;
using Confifu.Abstractions;

namespace Confifu.ConfigVariables
{
    /// <summary>
    /// IConfigVariables implementation. It tries to get Variable value from underlying IConfigVariables 
    /// list and returns the first not null variable returned by some IConfigVariables from the chain.
    /// </summary>
    public class ConfigVariablesChain : IConfigVariables
    {
        private readonly List<IConfigVariables> _chain;

        /// <summary>
        /// Creates new ConfigVariablesChain instance based on a given <para>chain</para>
        /// </summary>
        /// <param name="chain">IEnumerable of underlying IConfigVariables</param>
        public ConfigVariablesChain(IEnumerable<IConfigVariables> chain)
        {
            if (chain == null) throw new ArgumentNullException(nameof(chain));

            _chain = chain.ToList();
        }

        /// <summary>
        /// Returns first not null variable from underlying IConfigVariables chain
        /// </summary>
        /// <param name="key">key string (not null)</param>
        /// <returns>variable string or null</returns>
        public string this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                return _chain.Select(x => x[key]).FirstOrDefault(x => x != null);
            }
        }
    }
}