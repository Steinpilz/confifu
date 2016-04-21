namespace Confifu.Abstractions
{
    public interface IConfigVariables
    {
        string this[string key] { get; }
    }
}