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
    public class SloaderConfig
    {
        public SloaderConfig()
        {
            Secrets = new SloaderSecrets();
        }

        public async static Task<SloaderConfig> Load(string yamlLocation, Dictionary<string, string> secrets)
        {
            Trace.TraceInformation($"{nameof(SloaderConfig)} loading invoked for '{yamlLocation}'.");
            return await SloaderConfigLoader.GetAsync(yamlLocation, secrets);
        }

        public SloaderSecrets Secrets { get; set; }

        public CrawlerConfig Crawler { get; set; }

        public DropConfig Drop { get; set; }

    }
}