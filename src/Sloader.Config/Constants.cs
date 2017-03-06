using YamlDotNet.Serialization;

namespace Sloader.Config
{
    /// <summary>
    /// Common Constants used in the lib
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// YamlDeserializer, should make sure that the file itself is enough flexible to host other content as well.
        /// </summary>
        public static Deserializer SloaderYamlDeserializer => new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
    }

}