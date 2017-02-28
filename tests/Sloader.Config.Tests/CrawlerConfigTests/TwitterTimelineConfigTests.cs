using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Config.Tests.CrawlerConfigTests
{
    public class TwitterTimelineConfigTests
    {

        [Fact]
        public void Key_Is_Identifier()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  TwitterTimelinesToCrawl:");
            fakeYaml.AppendLine("  - Handle: codeinsideblog");
            fakeYaml.AppendLine("    Key: CIB");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.TwitterTimelinesToCrawl.First();

            Assert.Equal("CIB", configValue.Key);
        }

        [Fact]
        public void Default_For_IncludeRetweets_Is_False()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  TwitterTimelinesToCrawl:");
            fakeYaml.AppendLine("  - Handle: codeinsideblog");
            fakeYaml.AppendLine("    Key: CIB");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.TwitterTimelinesToCrawl.First();

            Assert.False(configValue.IncludeRetweets);
        }
    }
}
