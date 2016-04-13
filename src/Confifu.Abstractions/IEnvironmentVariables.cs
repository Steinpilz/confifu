namespace Confifu.Abstractions
{
    public interface IEnvironmentVariables
    {
        string this[string key] { get; }
        //string GetVariable(string key);
    }
}