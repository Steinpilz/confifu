using System;
using Confifu.Abstractions;

namespace Confifu
{
    public class EnvironmentConfigVariables : IConfigVariables
    {
        public string this[string key] => Environment.GetEnvironmentVariable(key);
    }
}