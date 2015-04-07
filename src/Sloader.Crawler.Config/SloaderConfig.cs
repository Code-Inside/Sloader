using System.Threading.Tasks;

namespace Sloader.Crawler.Config
{
    public class SloaderConfig
    {
        public async static Task<SloaderConfig> Load(string yamlLocation)
        {
            return await SloaderConfigLoader.GetAsync(yamlLocation);
        }

        public CrawlerConfig Crawler { get; set; }
    }
}