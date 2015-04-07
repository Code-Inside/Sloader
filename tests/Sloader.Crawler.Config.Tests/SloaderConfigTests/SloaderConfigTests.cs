using System.IO;
using Xunit;

namespace Sloader.Crawler.Config.Tests.SloaderConfigTests
{
    public class SloaderConfigTests
    {
        [Fact]
        public void Deserialize_Of_Testdata_To_Config_Works()
        {
            string yaml = TestHelperForCurrentProject.GetTestFileContent("SloaderConfigTests/Sample.yaml");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(yaml));

            Assert.NotNull(result.Crawler);
            Assert.Equal(3, result.Crawler.FeedsToCrawl.Count);
            Assert.Equal(3, result.Crawler.TwitterTimelinesToCrawl.Count);
        }


    }
}
