using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sloader.Crawler.Config
{
    public static class SloaderConfigLoader
    {
        public async static Task<SloaderConfig> GetAsync(string yamlLocation)
        {
            var client = new HttpClient();
            var configString = await client.GetStringAsync(yamlLocation);

            var deserializer = Constants.SloaderYamlDeserializer;
            var config = deserializer.Deserialize<SloaderConfig>(new StringReader(configString));

            return config;
        }
    }
}