using YamlDotNet.Serialization;

namespace Sloader.Config
{
    public static class Constants
    {
        public static Deserializer SloaderYamlDeserializer
        {
            get
            {
                return new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            }
        }
        
    }

}