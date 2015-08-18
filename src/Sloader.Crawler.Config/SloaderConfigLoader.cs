using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sloader.Crawler.Config
{
    public static class SloaderConfigLoader
    {
        public async static Task<SloaderConfig> GetAsync(string yamlLocation, Dictionary<string, string> secrets)
        {
            var client = new HttpClient();
            var configString = await client.GetStringAsync(yamlLocation);

            var config = SloaderConfigDeserializer.GetConfigWithEmbeddedSecrets(configString, secrets);

            return config;
        }
    }
}