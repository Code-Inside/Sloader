using YamlDotNet.Serialization;

namespace Sloader.Crawler.Config
{

    public static class Constants
    {
        public const string SloaderAzureBlobContainer = "sloader";
        public const string SloaderAzureBlobFileName = "data.json";

        public static Deserializer SloaderYamlDeserializer
        {
            get
            {
                return new Deserializer(ignoreUnmatched: true);
            }
        }
        
    }

}