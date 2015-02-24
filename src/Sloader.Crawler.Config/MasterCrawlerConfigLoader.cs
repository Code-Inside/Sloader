using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Sloader.Crawler.Config
{
    public static class MasterCrawlerConfigLoader
    {
        public async static Task<MasterCrawlerConfig> GetAsync(string yamlLocation)
        {
            var client = new HttpClient();
            var configString = await client.GetStringAsync(yamlLocation);

            var deserializer = Constants.SloaderYamlDeserializer;
            var config = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(configString));

            return config;
        }
    }
}