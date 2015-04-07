using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Crawler.Config.Tests.SloaderConfigTests
{
    public class FeedCrawlerConfigTests
    {
        [Fact]
        public void Default_For_LoadSocialLinkCounters_Is_True()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  FeedsToCrawl:");
            fakeYaml.AppendLine("  - Url: http://blogin.codeinside.eu/feed");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.FeedsToCrawl.First();

            Assert.True(configValue.LoadSocialLinkCounters);
        }

        [Fact]
        public void Url_Is_Identifier()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  FeedsToCrawl:");
            fakeYaml.AppendLine("  - Url: http://blogin.codeinside.eu/feed");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.FeedsToCrawl.First();

            Assert.Equal("http://blogin.codeinside.eu/feed", configValue.ResultIdentifier);
        }
    }
}
