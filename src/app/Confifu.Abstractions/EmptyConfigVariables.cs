namespace Confifu.Abstractions
{
    /// <summary>
    /// IConfigVariables implementation returning always null
    /// </summary>
    public class EmptyConfigVariables : IConfigVariables
    {
        /// <summary>
        /// Returns null as variable value
        /// </summary>
        /// <param name="key">key string</param>
        /// <returns>null</returns>
        public string this[string key] => null;
    }
}