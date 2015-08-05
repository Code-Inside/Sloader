using System.IO;
using System.Linq;
using System.Text;
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

        [Fact]
        public void Deserialize_Of_Secrets_In_Testdata_To_Config_Works()
        {
            string yaml = TestHelperForCurrentProject.GetTestFileContent("SloaderConfigTests/Sample.yaml");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(yaml));

            Assert.NotNull(result.Secrets);
            Assert.Equal("HelloWorldKey", result.Secrets.TwitterConsumerKey);
            Assert.Equal("HelloWorldSecret", result.Secrets.TwitterConsumerSecret);
        }

        [Fact]
        public void Secrets_Is_Never_Null()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  TwitterTimelinesToCrawl:");
            fakeYaml.AppendLine("  - Handle: codeinsideblog");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));

            Assert.NotNull(result.Secrets);
        }


    }
}
