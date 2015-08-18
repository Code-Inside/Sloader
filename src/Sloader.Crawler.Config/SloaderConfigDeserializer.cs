using System.Collections.Generic;
using System.IO;

namespace Sloader.Crawler.Config
{
    public static class SloaderConfigDeserializer
    {
        public static SloaderConfig GetConfigWithEmbeddedSecrets(string yamlString, Dictionary<string, string> secrets)
        {

            var deserializer = Constants.SloaderYamlDeserializer;
            var config = deserializer.Deserialize<SloaderConfig>(new StringReader(yamlString));

            return config;

        }
    }
}