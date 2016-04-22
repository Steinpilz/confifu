using System;
using System.Collections.Generic;
using Confifu.Abstractions;

namespace Confifu
{
    public class DictionaryConfigVariables : IConfigVariables
    {
        private readonly Dictionary<string, string> _vars;

        public DictionaryConfigVariables(Dictionary<string, string> dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            _vars = dict;
        }

        public string this[string key]
        {
            get
            {
                string ret;
                return _vars.TryGetValue(key, out ret) ? ret : null;
            }
        }
    }
}