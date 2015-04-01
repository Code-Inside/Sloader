using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Crawler.Config.Tests.MasterCrawlerConfigTests
{
    public class TwitterTimelineConfigTests
    {
      
        [Fact]
        public void Handle_Is_Identifier()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("TwitterTimelinesToCrawl:");
            fakeYaml.AppendLine("- Handle: codeinsideblog");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.TwitterTimelinesToCrawl.First();

            Assert.Equal("codeinsideblog", configValue.ResultIdentifier);
        }
    }
}
