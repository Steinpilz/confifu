using System;
using Confifu.Abstractions;

namespace Confifu.Samples.App
{
    public class CustomConfigVariables
        : IConfigVariables
    {
        public string this[string key] => Environment.GetEnvironmentVariable(key);
    }
}