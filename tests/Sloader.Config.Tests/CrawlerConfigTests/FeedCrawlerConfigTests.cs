using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Config.Tests.CrawlerConfigTests
{
    public class FeedCrawlerConfigTests
    {
        [Fact]
        public void Default_For_LoadSocialLinkCounters_Is_False()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  FeedsToCrawl:");
            fakeYaml.AppendLine("  - Url: http://blogin.codeinside.eu/feed");
            fakeYaml.AppendLine("    Key: Blog");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.FeedsToCrawl.First();

            Assert.False(configValue.LoadSocialLinkCounters);
        }

        [Fact]
        public void Key_Is_Identifier()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  FeedsToCrawl:");
            fakeYaml.AppendLine("  - Url: http://blogin.codeinside.eu/feed");
            fakeYaml.AppendLine("    Key: Blog");


            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.FeedsToCrawl.First();

            Assert.Equal("Blog", configValue.Key);
        }
    }
}
