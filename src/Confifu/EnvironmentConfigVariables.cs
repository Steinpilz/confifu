using System;
using Confifu.Abstractions;

namespace Confifu
{
    public class EnvironmentConfigVariables : IConfigVariables
    {
        public string this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                return Environment.GetEnvironmentVariable(key);
            }
        }
    }
}