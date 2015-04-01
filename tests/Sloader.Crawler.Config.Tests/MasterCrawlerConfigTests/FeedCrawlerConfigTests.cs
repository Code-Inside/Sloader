using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Crawler.Config.Tests.MasterCrawlerConfigTests
{
    public class FeedCrawlerConfigTests
    {
        [Fact]
        public void Default_For_LoadSocialLinkCounters_Is_True()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("FeedsToCrawl:");
            fakeYaml.AppendLine("- Url: http://blogin.codeinside.eu/feed");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.FeedsToCrawl.First();

            Assert.True(configValue.LoadSocialLinkCounters);
        }

        [Fact]
        public void Url_Is_Identifier()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("FeedsToCrawl:");
            fakeYaml.AppendLine("- Url: http://blogin.codeinside.eu/feed");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.FeedsToCrawl.First();

            Assert.Equal("http://blogin.codeinside.eu/feed", configValue.ResultIdentifier);
        }
    }
}
