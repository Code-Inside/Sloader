using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sloader.Config
{
    public static class SloaderConfigLoader
    {
        public async static Task<SloaderConfig> GetAsync(string yamlLocation, Dictionary<string, string> secrets)
        {
            string configString = string.Empty;

            if (yamlLocation.ToLowerInvariant().StartsWith("https://") ||
                yamlLocation.ToLowerInvariant().StartsWith("http://"))
            {
                var client = new HttpClient();
                configString = await client.GetStringAsync(yamlLocation);
            }
            else
            {
                configString = File.ReadAllText(yamlLocation);
            }

            var config = SloaderConfigLoader.Parse(configString, secrets);

            return config;
        }

        public static SloaderConfig Parse(string yamlConfig, Dictionary<string, string> secrets)
        {
            var config = SloaderConfigDeserializer.GetConfigWithEmbeddedSecrets(yamlConfig, secrets);

            return config;
        }
    }
}