namespace Confifu.Abstractions
{
    /// <summary>
    /// Describes Configuration Variables
    /// </summary>
    public interface IConfigVariables
    {
        /// <summary>
        /// Returns Config Vaiables by given key
        /// </summary>
        /// <param name="key">Config Variable Key</param>
        /// <returns>Config Variable (could be null)</returns>
        string this[string key] { get; }
    }
}