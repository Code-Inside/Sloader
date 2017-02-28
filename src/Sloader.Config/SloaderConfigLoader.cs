using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sloader.Config
{
    /// <summary>
    /// Can load the actual yaml file from a local filepath 
    /// or via the HttpClient and embeds the given secrets.
    /// </summary>
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

            var config = SloaderConfigDeserializer.GetConfigWithEmbeddedSecrets(configString, secrets);

            return config;
        }
    }
}