using System;
using System.Collections.Generic;
using Confifu.Abstractions;

namespace Confifu
{
    /// <summary>
    /// IConfigVariables impnementation returning
    /// variables from a given dictionary
    /// </summary>
    public class DictionaryConfigVariables : IConfigVariables
    {
        private readonly Dictionary<string, string> _vars;

        /// <summary>
        /// Creates new DictionaryConfigVariables instance based on a given <para>dict</para>
        /// </summary>
        /// <param name="dict">Dictionary instance</param>
        public DictionaryConfigVariables(Dictionary<string, string> dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            _vars = dict;
        }

        /// <summary>
        /// Returns dictionary value by given <para>key</para> or null if not found
        /// </summary>
        /// <param name="key">key string</param>
        /// <returns>variable string or null</returns>
        public string this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                string ret;
                return _vars.TryGetValue(key, out ret) ? ret : null;
            }
        }
    }
}