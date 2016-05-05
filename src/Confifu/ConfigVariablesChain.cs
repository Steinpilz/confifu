using System;
using System.Collections.Generic;
using System.Linq;
using Confifu.Abstractions;

namespace Confifu
{
    public class ConfigVariablesChain : IConfigVariables
    {
        private readonly List<IConfigVariables> _chain;

        public ConfigVariablesChain(IEnumerable<IConfigVariables> chain)
        {
            if (chain == null) throw new ArgumentNullException(nameof(chain));

            _chain = chain.ToList();
        }

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