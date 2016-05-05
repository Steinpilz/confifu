using Confifu.Abstractions;

namespace Confifu
{
    public class EmptyConfigVariables : IConfigVariables
    {
        public string this[string key] => null;
    }
}