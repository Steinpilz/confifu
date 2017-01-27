namespace Confifu.Abstractions
{
    /// <summary>
    /// Describes Application Configuration
    /// </summary>
    public interface IAppConfig
    {
        /// <summary>
        /// Returns a Config Option by given key
        /// </summary>
        /// <param name="key">Option key</param>
        /// <returns>Config Option instance (could be null)</returns>
        object this[string key] { get; set; }
    }
}