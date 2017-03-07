using System;

namespace Confifu.Abstractions
{

    public static class ConfigVariables
    {
        public static IConfigVariables Build(Action<ConfigVariablesBuilder> buildAction)
        {
            var builder = new ConfigVariablesBuilder();

            buildAction?.Invoke(builder);

            return builder.Build();
        }
    }
}