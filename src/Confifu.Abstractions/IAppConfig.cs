namespace Confifu.Abstractions
{
    public interface IAppConfig
    {
        object this[string key] { get; set; }
    }
}