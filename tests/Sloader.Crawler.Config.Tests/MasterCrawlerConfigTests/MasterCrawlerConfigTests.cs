using System.IO;
using Xunit;

namespace Sloader.Crawler.Config.Tests.MasterCrawlerConfigTests
{
    public class MasterCrawlerConfigTests
    {
        [Fact]
        public void Deserialize_Of_Testdata_To_MasterCrawlerConfig_Works()
        {
            string yaml = TestHelperForCurrentProject.GetTestFileContent("MasterCrawlerConfigTests/Sample.yaml");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(yaml));

            Assert.Equal(3, result.FeedsToCrawl.Count);
            Assert.Equal(3, result.TwitterTimelinesToCrawl.Count);
        }


    }
}
