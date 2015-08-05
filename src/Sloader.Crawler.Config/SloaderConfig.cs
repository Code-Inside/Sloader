using System.Threading.Tasks;

namespace Sloader.Crawler.Config
{
    public class SloaderConfig
    {
        public SloaderConfig()
        {
            Secrets = new SloaderSecrets();
        }

        public async static Task<SloaderConfig> Load(string yamlLocation)
        {
            return await SloaderConfigLoader.GetAsync(yamlLocation);
        }

        public SloaderSecrets Secrets { get; set; }

        public CrawlerConfig Crawler { get; set; }
    }
}