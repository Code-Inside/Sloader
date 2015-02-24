using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Sloader.Crawler.Config
{

    public static class Constants
    {
        public const string SloaderAzureBlobPath = "sloader/data.json";

        public static Deserializer SloaderYamlDeserializer
        {
            get
            {
                return new Deserializer(ignoreUnmatched: true);
            }
        }

        public static JsonSerializerSettings CrawlerJsonSerializerSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                };    
            }
        }
        
    }
}