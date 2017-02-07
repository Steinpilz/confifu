using Confifu.Abstractions;
using System;

namespace Confifu.Tests.ConfigVariables
{

    public class ConfigVariablesBuilderTestsBase
    {
        protected IConfigVariables Build(Action<ConfigVariablesBuilder> config)
        {
            var builder = new ConfigVariablesBuilder();
            config(builder);
            return builder.Build();
        }
    }
}