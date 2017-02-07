using Confifu.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Confifu.ConfigVariables
{
    public class LinkableConfigVariables : IConfigVariables
    {
        public string this[string key] 
            => string.IsNullOrEmpty(key) ? null : new ValueResolver(underlying, key).Resolve();

        readonly IConfigVariables underlying;

        public LinkableConfigVariables(IConfigVariables underlying)
        {
            this.underlying = underlying;
            if (underlying == null)
                throw new ArgumentNullException(nameof(underlying));
        }

        class ValueResolver
        {
            private readonly string originKey;
            private readonly IConfigVariables vars;
            private HashSet<string> visitedKeys;

            public ValueResolver(IConfigVariables vars, string key)
            {
                this.vars = vars;
                this.originKey = key;
            }

            private IEnumerable<string> BuildPrefixes(string key)
            {
                var lastIndex = key.Length-1;

                do
                {
                    lastIndex = key.LastIndexOf(':', lastIndex);
                    if (lastIndex >= 0)
                        yield return key.Substring(0, lastIndex + 1);
                    lastIndex--;
                }
                while (lastIndex >= 0);

                yield return "";
            }

            public string Resolve()
            {
                this.visitedKeys = new HashSet<string>();
                return GetValue(originKey);
            }

            private string GetValue(string key)
            {
                var underlyingValue = vars[key];
                if (underlyingValue == null)
                    return null;

                if (visitedKeys.Contains(key))
                    throw new InvalidOperationException($"Circular dependency detected. Keys visited: {string.Join(", ", visitedKeys)}");

                visitedKeys.Add(key);

                var possiblePrefixes = new Lazy<IEnumerable<string>>(() => BuildPrefixes(key));
                return new StringInterpolator(underlyingValue, x => ResolveVar(x, possiblePrefixes.Value)).Interpolate();
            }

            private string ResolveVar(string var, IEnumerable<string> possiblePrefixes)
            {
                return possiblePrefixes.Select(x => x + var).Select(GetValue).FirstOrDefault(x => x != null);
            }
            
        }
    }

    public static class LinkableConfigVariablesExts
    {
        /// <summary>
        /// Add linkable config variables
        /// </summary>
        /// <param name="builder">child builder which will be wrapped</param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ConfigVariablesBuilder Linkable(this ConfigVariablesBuilder builder, 
            Action<ConfigVariablesBuilder> config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var childBuilder = new ConfigVariablesBuilder();
            config(childBuilder);

            builder.AddBuilder(new LinkableConfigVariablesBuilder(childBuilder));

            return builder;
        }
    }

    public class LinkableConfigVariablesBuilder : IConfigVariablesBuilder
    {
        private readonly IConfigVariablesBuilder underlyingBuilder;

        public LinkableConfigVariablesBuilder(IConfigVariablesBuilder underlyingBuilder)
        {
            this.underlyingBuilder = underlyingBuilder;
        }

        public IConfigVariables Build()
        {
            return new LinkableConfigVariables(this.underlyingBuilder.Build());
        }
    }
}
