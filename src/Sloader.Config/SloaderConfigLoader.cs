using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sloader.Config
{
    /// <summary>
    /// Can load the actual yml file from a local filepath 
    /// or via the HttpClient and invokes the deserializer to embeds the given secrets
    /// </summary>
    public static class SloaderConfigLoader
    {
        public static async Task<SloaderConfig> GetAsync(string ymlLocation, Dictionary<string, string> secrets)
        {
            Trace.TraceInformation($"{nameof(SloaderConfigLoader)} invoked for '{ymlLocation}'.");

            string configString;

            if (ymlLocation.ToLowerInvariant().StartsWith("https://") ||
                ymlLocation.ToLowerInvariant().StartsWith("http://"))
            {
                var client = new HttpClient();
                configString = await client.GetStringAsync(ymlLocation);
            }
            else
            {
                configString = File.ReadAllText(ymlLocation);
            }

            var config = SloaderConfigDeserializer.GetConfigWithEmbeddedSecrets(configString, secrets);

            return config;
        }

        
    }
}