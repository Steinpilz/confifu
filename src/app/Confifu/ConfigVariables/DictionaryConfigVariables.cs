using System;
using System.Collections.Generic;
using Confifu.Abstractions;
using System.Linq;

namespace Confifu.ConfigVariables
{
    /// <summary>
    /// IConfigVariables impnementation returning
    /// variables from a given dictionary
    /// </summary>
    public class DictionaryConfigVariables : IConfigVariables
    {
        private readonly IDictionary<string, string> _vars;

        /// <summary>
        /// Creates new DictionaryConfigVariables instance based on a given <para>dict</para>
        /// </summary>
        /// <param name="dict">IDictionary instance</param>
        public DictionaryConfigVariables(IDictionary<string, string> dict)
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

    public static class Exts
    {
        /// <summary>
        /// Add DictionaryConfigVariables to <paramref name="builder"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ConfigVariablesBuilder Dictionary(this ConfigVariablesBuilder builder,
            Action<DictionaryConfigVariablesBuilder> config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var dictionaryBuilder = new DictionaryConfigVariablesBuilder();
            config(dictionaryBuilder);

            builder.AddBuilder(dictionaryBuilder);

            return builder;
        }

        /// <summary>
        /// Add static config variables to <paramref name="builder"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config">static config variables action</param>
        /// <returns></returns>
        public static ConfigVariablesBuilder Static(this ConfigVariablesBuilder builder, Action<Action<string, string>> config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Dictionary(dict =>
            {
                config((key, value) => dict.Add(key, value));
            });
        }
    }

    public class DictionaryConfigVariablesBuilder : IConfigVariablesBuilder
    {
        private List<IDictionary<string, string>> dictionaries = new List<IDictionary<string, string>>();

        public DictionaryConfigVariablesBuilder AddDictionary(IDictionary<string, string> dict)
        {
            dictionaries.Add(dict);
            return this;
        }

        public DictionaryConfigVariablesBuilder Add(string key, string value)
        {
            EnsureLastDictionary()[key] = value;
            return this;
        }

        private IDictionary<string, string> EnsureLastDictionary()
        {
            if (dictionaries.Any())
                return dictionaries.Last();

            var dict = new Dictionary<string, string>();
            dictionaries.Add(dict);
            return dict;
        }

        public IConfigVariables Build()
        {
            var result = new Dictionary<string, string>();
            foreach (var dict in dictionaries)
                foreach (var pair in dict)
                    result[pair.Key] = pair.Value;

            return new DictionaryConfigVariables(result);
        }
    }
}