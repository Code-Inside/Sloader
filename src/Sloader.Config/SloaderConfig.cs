using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Sloader.Config.Crawler;
using Sloader.Config.Drop;

namespace Sloader.Config
{
    /// <summary>
    /// Root config entry, holds secrets, drops, crawler, etc.
    /// </summary>
    /// <example>
    /// Demo yml config (only top level elements):
    /// <code>
    /// Secrets:
    ///   ...
    /// 
    /// Crawler:
    ///   ...
    /// 
    /// Drop:
    ///   ...
    /// </code>
    /// </example>
    public class SloaderConfig
    {
        public SloaderConfig()
        {
            Secrets = new SloaderSecrets();
        }

        /// <summary>
        /// Loads and instantiate the config from a given Yml-file.
        /// </summary>
        /// <param name="ymlLocation">yml-file location as FilePath or URL</param>
        /// <param name="secrets">List of secret replacements</param>
        /// <returns></returns>
        public static async Task<SloaderConfig> Load(string ymlLocation, Dictionary<string, string> secrets)
        {
            Trace.TraceInformation($"{nameof(SloaderConfig)} loading invoked for '{ymlLocation}'.");
            return await SloaderConfigLoader.GetAsync(ymlLocation, secrets);
        }

        /// <summary>
        /// Common list of secrets used for multiple drops or crawlers.
        /// </summary>
        public SloaderSecrets Secrets { get; set; }

        /// <summary>
        /// List of all crawlers that are configured.
        /// </summary>
        public CrawlerConfig Crawler { get; set; }

        /// <summary>
        /// List of all drops that are configured.
        /// </summary>
        public DropConfig Drop { get; set; }

    }
}