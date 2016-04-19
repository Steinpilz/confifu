namespace Confifu.Abstractions
{
    public interface IConfigVariables
    {
        string this[string key] { get; }
        //string GetVariable(string key);
    }
}