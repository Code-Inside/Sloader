using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Sloader.Crawler.Config
{
    public static class MasterCrawlerConfigLoader
    {
        public static Deserializer SloaderYamlDeserializer
        {
            get
            {
                return new Deserializer(ignoreUnmatched: true);
            }
        }

        public async static Task<MasterCrawlerConfig> GetAsync(string yamlLocation)
        {
            var client = new HttpClient();
            var configString = await client.GetStringAsync(yamlLocation);

            var deserializer = SloaderYamlDeserializer;
            var config = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(configString));

            return config;
        }
    }
}