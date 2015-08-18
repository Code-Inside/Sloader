using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sloader.Crawler.Config
{
    public class SloaderConfig
    {
        public SloaderConfig()
        {
            Secrets = new SloaderSecrets();
        }

        public async static Task<SloaderConfig> Load(string yamlLocation, Dictionary<string, string> secrets)
        {
            return await SloaderConfigLoader.GetAsync(yamlLocation, secrets);
        }

        public SloaderSecrets Secrets { get; set; }

        public CrawlerConfig Crawler { get; set; }
    }
}