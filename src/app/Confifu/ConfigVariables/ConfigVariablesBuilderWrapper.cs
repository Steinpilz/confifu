using Confifu.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Confifu.ConfigVariables
{
    public class ConfigVariablesBuilderWrapper : IConfigVariablesBuilder
    {
        private IConfigVariablesBuilder underlying;
        private Func<IConfigVariables, IConfigVariables> wrapperFunc;
        
        public ConfigVariablesBuilderWrapper(
            IConfigVariablesBuilder underlying, 
            Func<IConfigVariables, IConfigVariables> wrapper
            )
        {
            this.underlying = underlying ?? throw new ArgumentNullException(nameof(underlying));
            this.wrapperFunc = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
        }

        public IConfigVariables Build()
            => this.wrapperFunc(this.underlying.Build());
    }

    public static class ConfigVariablesBuilderWrapperExts
    {
        public static IConfigVariablesBuilder Wrap(this IConfigVariablesBuilder builder, Func<IConfigVariables, IConfigVariables> wrapper)
            => new ConfigVariablesBuilderWrapper(builder ?? throw new ArgumentNullException(nameof(builder)), wrapper);

        public static ConfigVariablesBuilder ChildBuilder(this ConfigVariablesBuilder builder, 
            Action<ConfigVariablesBuilder> childBuilderAction,
            Func<IConfigVariables, IConfigVariables> wrapper = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var childBuilder = new ConfigVariablesBuilder();

            (childBuilderAction ?? throw new ArgumentNullException(nameof(childBuilderAction)))
                (childBuilder);

            return builder.AddBuilder(wrapper == null ? childBuilder : childBuilder.Wrap(wrapper));
        }

        public static ConfigVariablesBuilder Prefixed(this ConfigVariablesBuilder builder,
            string prefix,
            Action<ConfigVariablesBuilder> configurator
            )
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));

            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            return builder.ChildBuilder(configurator, x => x.WithPrefix(prefix));
        }
    }

}
