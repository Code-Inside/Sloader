using System.IO;
using System.Threading.Tasks;
using Sloader.Crawler.Config;
using Xunit;
using YamlDotNet.Serialization;

namespace Sloader.Tests.MasterCrawlerConfigTests
{
    public class MasterCrawlerConfigTests
    {
        [Fact]
        public void Deserialize_Of_Testdata_To_MasterCrawlerConfig_Works()
        {
            string yaml = TestHelperForCurrentProject.GetTestFileContent("MasterCrawlerConfigTests.Sample.yaml");

            var deserializer = MasterCrawlerConfigLoader.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(yaml));

            Assert.Equal(3, result.FeedsToCrawl.Count);
            Assert.Equal(3, result.TwitterTimelinesToCrawl.Count);
        }


    }
}
